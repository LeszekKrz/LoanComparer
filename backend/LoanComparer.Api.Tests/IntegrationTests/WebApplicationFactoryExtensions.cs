using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    internal static class WebApplicationFactoryExtensions
    {
        internal static async Task DoWithinScope<TService>(
            this WebApplicationFactory<Program> webApplicationFactory, Func<TService, Task> action)
        where TService : notnull
        {
            using var serviceScope = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var service = serviceScope.ServiceProvider.GetRequiredService<TService>();
            await action(service);
        }

        internal static async Task DoWithinScope<TService, UService>(
            this WebApplicationFactory<Program> webApplicationFactory, Func<TService, UService, Task> action)
            where TService : notnull
            where UService : notnull
        {
            using var serviceScope = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var tService = serviceScope.ServiceProvider.GetRequiredService<TService>();
            var uService = serviceScope.ServiceProvider.GetRequiredService<UService>();
            await action(tService, uService);
        }
    }
}
