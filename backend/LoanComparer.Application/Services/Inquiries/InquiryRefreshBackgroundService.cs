using LoanComparer.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LoanComparer.Application.Services.Inquiries;

public class InquiryRefreshBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<InquiryConfiguration> _config;
    private PeriodicTimer? _timer;

    public InquiryRefreshBackgroundService(IServiceScopeFactory scopeFactory,
        IOptionsMonitor<InquiryConfiguration> config)
    {
        _scopeFactory = scopeFactory;
        _config = config;

        var previousConfig = config.CurrentValue;
        _config.OnChange(newConfig =>
        {
            if (previousConfig == newConfig) return;
            _timer?.Dispose();
            _timer = new PeriodicTimer(newConfig.RefreshInterval);
        });
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new PeriodicTimer(_config.CurrentValue.RefreshInterval);
        while (!stoppingToken.IsCancellationRequested)
        {
            await RefreshAllStatusesAsync();
            await _timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task RefreshAllStatusesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var refresher = scope.ServiceProvider.GetRequiredService<IInquiryRefresher>();
        await refresher.RefreshAllStatusesAndSendNotificationsAsync();
    }
}