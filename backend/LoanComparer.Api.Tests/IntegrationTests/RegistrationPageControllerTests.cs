using FluentAssertions;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Respawn;
using System.Net;
using System.Text;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    [TestClass]
    [TestCategory(TestCategories.SQL)]

    public class RegistrationPageControllerTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory;
        private string _connection;
        private Respawner _respawner;
        private Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();
        private Mock<UserManager<User>> _userManagerMock = new Mock<UserManager<User>>();

        [TestInitialize]
        public async Task Initialize()
        {
            _webApplicationFactory = IntegrationTestSetup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            _connection = config.GetConnectionString("Database");
            _respawner = await IntegrationTestSetup.CreateRespawnerAsync(_connection);
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).Verifiable();
            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync("token").Verifiable();
        }

        [TestMethod]
        public async Task RegisterUser_ShouldCreateNewUserAndReturnCreated()
        {
            // ARRANGE

            var httpClient = _webApplicationFactory.CreateClient();

            await _respawner.ResetAsync(_connection);

            var userForRegistrationDTO = new UserForRegistrationDTO(
                "testFirstName",
                "testLastName",
                "marcin.latoszek06@gmail.com",
                new JobTypeDTO("Other"),
                1,
                new GovernmentIdDTO(
                    "PESEL",
                    "05240816772"),
                "Password123!",
                "Password123!",
                "https://testClientURI");

            var stringContent = new StringContent(JsonConvert.SerializeObject(userForRegistrationDTO), Encoding.UTF8, "application/json");

            // ACT

            HttpResponseMessage response = await httpClient.PostAsync("api/registration-page/register", stringContent);

            // ASSERT

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    User? userResult = await userManager
                        .Users
                        .Include(x => x.JobType)
                        .Include(x => x.GovernmentId)
                        .Include(x => x.UserRoles)
                            .ThenInclude(x => x.Role)
                        .SingleOrDefaultAsync();

                    userResult.Should().NotBe(null);
                    userResult.FirstName.Should().Be(userForRegistrationDTO.FirstName);
                    userResult.LastName.Should().Be(userForRegistrationDTO.LastName);
                    userResult.UserName.Should().Be(userForRegistrationDTO.Email);
                    userResult.NormalizedUserName.Should().Be(userForRegistrationDTO.Email.ToUpper());
                    userResult.Email.Should().Be(userForRegistrationDTO.Email);
                    userResult.NormalizedUserName.Should().Be(userForRegistrationDTO.Email.ToUpper());
                    userResult.JobType.Name.Should().Be(userForRegistrationDTO.JobType.Name);
                    userResult.IncomeLevel.Should().Be(userForRegistrationDTO.IncomeLevel);
                    userResult.GovernmentId.Type.Should().Be(userForRegistrationDTO.GovernmentId.Type);
                    userResult.GovernmentId.Value.Should().Be(userForRegistrationDTO.GovernmentId.Value);
                    userManager.PasswordHasher.VerifyHashedPassword(userResult, userResult.PasswordHash, userForRegistrationDTO.Password);
                    userResult
                        .UserRoles
                        .Select(userRole => (userRole.Role.Name, userRole.Role.NormalizedName))
                        .SingleOrDefault()
                        .Should().Be((LoanComparerConstants.ClientRoleName, LoanComparerConstants.ClientRoleName.ToUpper()));
                    userResult.EmailConfirmed.Should().Be(false);
                });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            _userManagerMock.Verify(userManager => userManager.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Once);
            _emailServiceMock.Verify(emailService => emailService.SendEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
