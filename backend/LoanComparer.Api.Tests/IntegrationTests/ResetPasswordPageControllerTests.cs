using FluentAssertions;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Respawn;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LoanComparer.Application;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    [TestClass]
    [TestCategory(TestCategories.SQL)]
    public class ResetPasswordPageControllerTests
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
        public async Task RegisterUser_ShouldCreateNewUserAndReturnCreated()
        {
            // ARRANGE

            var httpClient = _webApplicationFactory.CreateClient();

            await _respawner.ResetAsync(_connection);

            ResetPasswordDTO resetPasswordDTO = null;

            await _webApplicationFactory.DoWithinScope<LoanComparerContext, UserManager<User>>(
                async (context, userManager) =>
                {
                    var user = new User(
                        "testFirstName",
                        "testLastName",
                        "loan.comparer@gmail.com",
                        await context.JobTypes.SingleAsync(jobType => jobType.Name == "Other"),
                        1,
                        new GovernmentIdEntity(
                            "PESEL",
                            "05240816772"));
                    await userManager.CreateAsync(user, "oldPassword123!");
                    User userFromDb = await userManager.FindByEmailAsync(user.Email);
                    string token = await userManager.GeneratePasswordResetTokenAsync(userFromDb);
                    resetPasswordDTO = new ResetPasswordDTO(
                        "newPassword123!",
                        "newPassword123!",
                        "loan.comparer@gmail.com",
                        HttpUtility.UrlEncode(token));
                });

            var stringContent = new StringContent(JsonConvert.SerializeObject(resetPasswordDTO), Encoding.UTF8, "application/json");

            // ACT

            HttpResponseMessage response = await httpClient.PostAsync("api/reset-password-page/reset-password", stringContent);

            // ASSERT

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    User? userResult = await userManager.Users.SingleOrDefaultAsync();

                    userResult.Should().NotBe(null);
                    userManager.PasswordHasher.VerifyHashedPassword(userResult, userResult.PasswordHash, resetPasswordDTO.Password);
                });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
