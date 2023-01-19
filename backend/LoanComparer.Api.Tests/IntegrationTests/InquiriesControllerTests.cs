using System.Net;
using FluentAssertions;
using Flurl.Http;
using LoanComparer.Application;
using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.JwtFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Respawn;

namespace LoanComparer.Api.Tests.IntegrationTests;

[TestClass]
[TestCategory(TestCategories.SQL)]
public sealed class InquiriesControllerTests
{
        private WebApplicationFactory<Program> _webApplicationFactory;
        private string _connectionString;
        private Respawner _respawner;

        [TestInitialize]
        public async Task Initialize()
        {
            _webApplicationFactory = IntegrationTestSetup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetRequiredService<IConfiguration>();
            _connectionString = config.GetConnectionString("Database");
            _respawner = await IntegrationTestSetup.CreateRespawnerAsync(_connectionString);
        }

        [TestMethod]
        public async Task CreateInquiry_ShouldSucceedForAnonymousUser()
        {
            var client = new FlurlClient(_webApplicationFactory.CreateClient()).
                AllowAnyHttpStatus();
            await _respawner.ResetAsync(_connectionString);

            var response = await client.Request("api", "inquiries").PostJsonAsync(new InquiryRequest
            {
                AmountRequested = 10_000m,
                NumberOfInstallments = 10,
                PersonalData = new()
                {
                    FirstName = "test",
                    LastName = "user",
                    NotificationEmail = "test@user.com",
                    BirthDate = null
                },
                JobDetails = new()
                {
                    JobName = "Director",
                    Description = "Director",
                    StartDate = null,
                    EndDate = null,
                    IncomeLevel = 5000m
                },
                GovernmentId = new("PESEL", "12345678910")
            });

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            var inquiry = await response.GetJsonAsync<InquiryResponse>();
            inquiry.Should().BeEquivalentTo(new InquiryResponse
                {
                    Id = Guid.Empty,
                    AmountRequested = 10_000m,
                    NumberOfInstallments = 10,
                    PersonalData = new()
                    {
                        FirstName = "test",
                        LastName = "user",
                        NotificationEmail = "test@user.com",
                        BirthDate = null
                    },
                    JobDetails = new()
                    {
                        JobName = "Director",
                        Description = "Director",
                        StartDate = null,
                        EndDate = null,
                        IncomeLevel = 5000m
                    },
                    GovtId = new("PESEL", "12345678910")
                },
                options => options.Excluding(i => i.Id).Excluding(i => i.CreationTime));
            inquiry.CreationTime.Should().BeBefore(DateTimeOffset.Now).And
                .BeAfter(DateTimeOffset.Now - TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public async Task CreateInquiry_ShouldUseValidation()
        {
            var client = new FlurlClient(_webApplicationFactory.CreateClient()).
                AllowAnyHttpStatus();
            await _respawner.ResetAsync(_connectionString);

            var response = await client.Request("api", "inquiries").PostJsonAsync(new InquiryRequest
            {
                AmountRequested = 10_000m,
                NumberOfInstallments = 10,
                PersonalData = new()
                {
                    FirstName = "test",
                    LastName = "user",
                    NotificationEmail = "testUser.com",
                    BirthDate = null
                },
                JobDetails = new()
                {
                    JobName = "invalid job name",
                    Description = "Director",
                    StartDate = null,
                    EndDate = null,
                    IncomeLevel = 5000m
                },
                GovernmentId = new("PESEL", "123456789")
            });

            response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var errors = await response.GetJsonAsync<IEnumerable<ErrorResponseDTO>>();
            errors.Should().HaveCount(4);   // Invalid government ID produces 2 errors
        }

        [TestMethod]
        public async Task GetStatusesForInquiry_ShouldReturnStatuses()
        {
            var client = new FlurlClient(_webApplicationFactory.CreateClient()).
                WithOAuthBearerToken(JwtHandler.GenerateTestToken("testUser")).
                AllowAnyHttpStatus();
            await _respawner.ResetAsync(_connectionString);

            using var scope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LoanComparerContext>();
            var inquiry = new Inquiry
            {
                AmountRequested = 10_000m,
                NumberOfInstallments = 10,
                PersonalData = new()
                {
                    FirstName = "test",
                    LastName = "user",
                    NotificationEmail = "testUser",
                    BirthDate = null
                },
                JobDetails = new()
                {
                    JobName = "Director",
                    Description = "Director",
                    StartDate = null,
                    EndDate = null,
                    IncomeLevel = 5000m
                },
                GovernmentId = new("PESEL", "12345678910")
            };
            var inquiryEntity = inquiry.ToEntity();
            var offerEntity = new Offer
            {
                Id = Guid.NewGuid(),
                LoanValue = inquiry.AmountRequested,
                NumberOfInstallments = inquiry.NumberOfInstallments,
                MonthlyInstallment = inquiry.AmountRequested / 2,
                Percentage = 50,
                DocumentLink = "https://testoffer",
            }.ToEntity();
            context.Inquiries.Add(inquiryEntity);
            context.Offers.Add(offerEntity);
            var statuses = new[]
            {
                new SentInquiryStatusEntity
                {
                    Id = Guid.NewGuid(),
                    BankName = "TestBank",
                    Inquiry = inquiryEntity,
                    InquiryId = inquiryEntity.Id,
                    Offer = null,
                    OfferId = null,
                    Status = InquiryStatus.Rejected
                },
                new SentInquiryStatusEntity
                {
                    Id = Guid.NewGuid(),
                    BankName = "TestBank",
                    Inquiry = inquiryEntity,
                    InquiryId = inquiryEntity.Id,
                    Offer = offerEntity,
                    OfferId = offerEntity.Id,
                    Status = InquiryStatus.OfferReceived
                }
            };
            context.InquiryStatuses.AddRange(statuses);
            await context.SaveChangesAsync();

            var response = await client.Request("api", "inquiries", inquiryEntity.Id, "offers").GetAsync();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            var offersResponse = await response.GetJsonAsync<IReadOnlyList<SentInquiryStatusDTO>>();
            offersResponse.Should().HaveCount(2);
            offersResponse.Should().BeEquivalentTo(new[]
            {
                new SentInquiryStatusDTO
                {
                    BankName = statuses[0].BankName,
                    Status = "REJECTED",
                    Offer = null
                },
                new SentInquiryStatusDTO
                {
                    BankName = statuses[1].BankName,
                    Status = "OFFERRECEIVED",
                    Offer = Offer.FromEntity(offerEntity).ToDto()
                }
            });
        }
}