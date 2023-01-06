using LoanComparer.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LoanComparer.Application.Services.Inquiries;

public class InquiryCleanupBackgroundService: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<InquiryConfiguration> _config;
    private PeriodicTimer? _timer;

    public InquiryCleanupBackgroundService(IServiceScopeFactory scopeFactory,
        IOptionsMonitor<InquiryConfiguration> config)
    {
        _scopeFactory = scopeFactory;
        _config = config;

        var previousConfig = config.CurrentValue;
        _config.OnChange(newConfig =>
        {
            if (previousConfig == newConfig) return;
            _timer?.Dispose();
            _timer = new PeriodicTimer(newConfig.CleanupInterval);
        });
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new PeriodicTimer(_config.CurrentValue.CleanupInterval);
        while (!stoppingToken.IsCancellationRequested)
        {
            await MarkOldStatusesAsTimeoutAsync();
            await _timer.WaitForNextTickAsync(stoppingToken);
        }
    }
    
    private async Task MarkOldStatusesAsTimeoutAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var refresher = scope.ServiceProvider.GetRequiredService<IInquiryRefresher>();
        await refresher.MarkOldStatusesAsTimeoutAsync();
    }
}