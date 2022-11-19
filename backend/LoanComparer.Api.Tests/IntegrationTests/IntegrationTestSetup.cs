using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
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
                });
            });
        }
    }
}
