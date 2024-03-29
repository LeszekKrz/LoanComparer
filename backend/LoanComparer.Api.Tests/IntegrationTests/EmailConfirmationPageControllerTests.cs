﻿using FluentAssertions;
using LoanComparer.Application.Model;
using LoanComparer.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Respawn;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Web;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    [TestClass]
    [TestCategory(TestCategories.SQL)]
    public class EmailConfirmationPageControllerTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory;
        private string _connection;
        private Respawner _respawner;

        [TestInitialize]
        public async Task Initialize()
        {
            _webApplicationFactory = IntegrationTestSetup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            _connection = config.GetConnectionString("Database");
            _respawner = await IntegrationTestSetup.CreateRespawnerAsync(_connection);
        }

        [TestMethod]
        public async Task ConfirmEmail_ShouldReturnOk()
        {
            // ARRANGE

            var httpClient = _webApplicationFactory.CreateClient();

            await _respawner.ResetAsync(_connection);

            string jobTypeName = "Other";
            string userPassword = "Password123!";
            string userEmail = "loan.comparer@gmail.com";
            string token= String.Empty;

            await _webApplicationFactory.DoWithinScope<LoanComparerContext, UserManager<User>>(
                async (context, userManager) =>
                {
                    var user = new User(
                        "testFirstName",
                        "testLastName",
                        userEmail,
                        await context.JobTypes.SingleAsync(jobType => jobType.Name == jobTypeName),
                        1,
                        new GovernmentIdEntity(
                            "PESEL",
                            "05240816772"));
                    await userManager.CreateAsync(user, userPassword);
                    User userFromDb = await userManager.FindByEmailAsync(user.Email);
                    token = await userManager.GenerateEmailConfirmationTokenAsync(userFromDb);
                });

            var queryParameters = new Dictionary<string, string?>()
            {
                { "token", HttpUtility.UrlEncode(token) },
                { "email", userEmail }
            };

            var uri = QueryHelpers.AddQueryString("api/confirm-email-page/confirm-email", queryParameters);


            // ACT

            var response = await httpClient.GetAsync(uri);

            // ASSERT

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    User userResult = await userManager.Users.SingleAsync();
                    userResult.EmailConfirmed.Should().Be(true);
                });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
