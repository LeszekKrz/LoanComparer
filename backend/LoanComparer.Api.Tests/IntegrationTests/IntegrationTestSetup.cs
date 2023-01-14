using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Respawn;
using Respawn.Graph;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    internal static class IntegrationTestSetup
    {
        internal static async Task<Respawner> CreateRespawnerAsync(string connectionString)
        {
            return await Respawner.CreateAsync(
                connectionString,
                new RespawnerOptions
                {
                    TablesToIgnore = new Table[]
                    {
                        "__EFMigrationsHistory",
                        "JobTypes",
                        "AspNetRoles"
                    }
                });
        }

        internal static WebApplicationFactory<Program> GetWebApplicationFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, @"..\..\..\appsettings.test.json");

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                Environment.SetEnvironmentVariable("SENDGRID_API_KEY", "TestSendGridAPIKey");
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddJsonFile(configPath, false);
                }).ConfigureServices(services =>
                {
                    services.RemoveAll<IHostedService>();
                    services.RemoveAll<IBankInterfaceFactory>();
                    services.AddScoped<IBankInterfaceFactory, TestBankApplicationFactory>();
                });
            });
        }

        internal static WebApplicationFactory<Program> GetWebApplicationFactory(IEmailService emailService)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, @"..\..\..\appsettings.test.json");

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddJsonFile(configPath, false);
                });
                builder.ConfigureServices(services =>
                {
                    services.Replace(new ServiceDescriptor(typeof(IEmailService), emailService));
                    services.RemoveAll<IHostedService>();
                    services.RemoveAll<IBankInterfaceFactory>();
                    services.AddScoped<IBankInterfaceFactory, TestBankApplicationFactory>();
                });
            });
        }

        private class TestBankApplicationFactory : IBankInterfaceFactory
        {
            public IReadOnlyList<IBankInterface> CreateBankInterfaces()
            {
                return new[] { new TestBankInterface() };
            }

            private class TestBankInterface : IBankInterface
            {
                public string BankName => "TestBank";
                public Task<SentInquiryStatus> RefreshStatusAsync(SentInquiryStatus status)
                {
                    return Task.FromResult(new SentInquiryStatus
                    {
                        Id = status.Id,
                        BankName = status.BankName,
                        Inquiry = status.Inquiry,
                        ReceivedOffer = status.ReceivedOffer,
                        Status = status.Status
                    });
                }

                public Task<SentInquiryStatus> SendInquiryAsync(Inquiry inquiry)
                {
                    return Task.FromResult(new SentInquiryStatus
                    {
                        Id = Guid.NewGuid(),
                        BankName = BankName,
                        Inquiry = inquiry,
                        ReceivedOffer = null,
                        Status = InquiryStatus.Pending
                    });
                }
            }
        }
    }
}
