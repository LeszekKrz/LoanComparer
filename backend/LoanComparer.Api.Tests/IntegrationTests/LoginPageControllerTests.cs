using FluentAssertions;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Respawn;
using System.Net;
using System.Text;
using LoanComparer.Application;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    [TestClass]
    [TestCategory(TestCategories.SQL)]
    public class LoginPageControllerTests
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
        public async Task LoginUser_ShouldReturnOk()
        {
            // ARRANGE

            var httpClient = _webApplicationFactory.CreateClient();

            await _respawner.ResetAsync(_connection);

            string jobTypeName = "Other";
            string userPassword = "Password123!";
            string userEmail = "loan.comparer@gmail.com";

            await _webApplicationFactory.DoWithinScope<LoanComparerContext, UserManager<User>>(
                async (context, userManager) =>
                {
                    var user = new User(
                        "testFirstName",
                        "testLastName",
                        userEmail,
                        await context.JobTypes.SingleAsync(jobType => jobType.Name == jobTypeName),
                        1,
                        new GovernmentId(
                            "PESEL",
                            "05240816772"));
                    await userManager.CreateAsync(user, userPassword);
                    User userFromDb = await userManager.FindByEmailAsync(user.Email);
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(userFromDb);
                    await userManager.ConfirmEmailAsync(userFromDb, token);
                });

            var userForAuthenticationDTO = new UserForAuthenticationDTO(userEmail, userPassword);

            var stringContent = new StringContent(JsonConvert.SerializeObject(userForAuthenticationDTO), Encoding.UTF8, "application/json");

            // ACT

            HttpResponseMessage response = await httpClient.PostAsync("api/login-page/login", stringContent);

            // ASSERT

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
