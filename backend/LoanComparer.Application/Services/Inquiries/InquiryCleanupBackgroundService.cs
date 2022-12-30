using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LoanComparer.Application.Services.Inquiries;

public class InquiryCleanupBackgroundService: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InquiryCleanupBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: Take cleanup interval from config
        using var timer = new PeriodicTimer(TimeSpan.FromHours(12));
        while (!stoppingToken.IsCancellationRequested)
        {
            await MarkOldStatusesAsTimeoutAsync();
            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }
    
    private async Task MarkOldStatusesAsTimeoutAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var refresher = scope.ServiceProvider.GetRequiredService<IInquiryRefresher>();
        await refresher.MarkOldStatusesAsTimeoutAsync();
    }
}