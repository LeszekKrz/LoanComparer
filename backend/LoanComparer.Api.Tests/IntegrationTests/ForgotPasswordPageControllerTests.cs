using FluentAssertions;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Respawn;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LoanComparer.Application;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    [TestClass]
    [TestCategory(TestCategories.SQL)]
    public class ForgotPasswordPageControllerTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory;
        private string _connection;
        private Respawner _respawner;
        private Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();

        [TestInitialize]
        public async Task Initialize()
        {
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).Verifiable();
            _webApplicationFactory = IntegrationTestSetup.GetWebApplicationFactory(_emailServiceMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            _connection = config.GetConnectionString("Database");
            _respawner = await IntegrationTestSetup.CreateRespawnerAsync(_connection);
        }

        [TestMethod]
        public async Task ForgotPassword_ShouldSendEmailAndOk()
        {
            // ARRANGE

            var httpClient = _webApplicationFactory.CreateClient();

            await _respawner.ResetAsync(_connection);

            var forgotPasswordDTO = new ForgotPasswordDTO("loan.comparer@gmail.com", "https://testURI");

            await _webApplicationFactory.DoWithinScope<LoanComparerContext, UserManager<User>>(
                async (context, userManager) =>
                {
                    var user = new User(
                        "testFirstName",
                        "testLastName",
                        forgotPasswordDTO.Email,
                        await context.JobTypes.SingleAsync(jobType => jobType.Name == "Other"),
                        1,
                        new GovernmentIdEntity(
                            "PESEL",
                            "05240816772"));
                    await userManager.CreateAsync(user, "Password123!");
                });

            var stringContent = new StringContent(JsonConvert.SerializeObject(forgotPasswordDTO), Encoding.UTF8, "application/json");

            // ACT

            HttpResponseMessage response = await httpClient.PostAsync("api/forgot-password-page/forgot-password", stringContent);

            // ASSERT

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            _emailServiceMock.Verify(emailService => emailService.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
