using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LoanComparer.Application.Services.Inquiries;

public class InquiryRefreshBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InquiryRefreshBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: Take refresh interval from config
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (!stoppingToken.IsCancellationRequested)
        {
            await RefreshAllStatusesAsync();
            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task RefreshAllStatusesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var refresher = scope.ServiceProvider.GetRequiredService<IInquiryRefresher>();
        await refresher.RefreshAllStatusesAndSendNotificationsAsync();
    }
}