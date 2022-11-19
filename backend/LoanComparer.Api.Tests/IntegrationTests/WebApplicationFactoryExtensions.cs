using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace LoanComparer.Api.Tests.IntegrationTests
{
    internal static class WebApplicationFactoryExtensions
    {
        internal static async Task DoWithinScope<TService>(
            this WebApplicationFactory<Program> webApplicationFactory,
            Func<TService, Task> action)
        {
            using var serviceScope = webApplicationFactory.Services.GetService<IServiceScopeFactory>().CreateScope();
            var service = serviceScope.ServiceProvider.GetService<TService>();
            await action(service);
        }

        internal static async Task DoWithinScope<TService, UService>(
            this WebApplicationFactory<Program> webApplicationFactory,
            Func<TService, UService, Task> action)
        {
            using var serviceScope = webApplicationFactory.Services.GetService<IServiceScopeFactory>().CreateScope();
            var tService = serviceScope.ServiceProvider.GetService<TService>();
            var uService = serviceScope.ServiceProvider.GetService<UService>();
            await action(tService, uService);
        }
    }
}
