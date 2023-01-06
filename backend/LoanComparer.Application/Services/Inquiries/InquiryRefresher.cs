using LoanComparer.Application.Configuration;
using LoanComparer.Application.Model;
using Microsoft.Extensions.Options;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class InquiryRefresher : IInquiryRefresher
{
    private readonly IReadOnlyList<IBankApiRefresher> _refreshers;
    private readonly IInquiryQuery _query;
    private readonly IInquiryCommand _command;
    private readonly IOptionsMonitor<InquiryConfiguration> _config;

    public InquiryRefresher(IEnumerable<IBankApiRefresher> refreshers, IInquiryQuery query, IInquiryCommand command,
        IOptionsMonitor<InquiryConfiguration> config)
    {
        _refreshers = refreshers.ToList();
        _query = query;
        _command = command;
        _config = config;
    }
    
    public async Task RefreshStatusesForUserAsync(string username)
    {
        var statusesToRefresh = await _query.GetPendingStatusesForUserAsync(username);
        foreach (var status in statusesToRefresh)
        {
            var refresher = GetRefresherForStatus(status);
            await refresher.RefreshStatusAsync(status);
        }
    }

    public async Task MarkOldStatusesAsTimeoutAsync()
    {
        var oldStatuses = await _query.GetPendingStatusesOlderThanAsync(_config.CurrentValue.TimeoutInterval);
        foreach (var status in oldStatuses)
        {
            await _command.MarkAsTimeoutAsync(status);
        }
    }

    public async Task RefreshAllStatusesAndSendNotificationsAsync()
    {
        var statusesToRefresh = await _query.GetAllPendingStatusesAsync();
        foreach (var inquiryStatus in statusesToRefresh)
        {
            var refresher = GetRefresherForStatus(inquiryStatus);
            var previousStatus = inquiryStatus.Status;
            var updated = await refresher.RefreshStatusAsync(inquiryStatus);
            if(updated.Status == previousStatus) continue;
            // TODO: Send mail
        }
    }

    private IBankApiRefresher GetRefresherForStatus(SentInquiryStatus status)
    {
        var refresher = _refreshers.FirstOrDefault(r => r.BankId == status.BankId);
        if (refresher is null)
            throw new InvalidOperationException(
                $"There is no refresher with bank id {status.BankId}, but status with id {status.Id} references it");
        return refresher;
    }
}