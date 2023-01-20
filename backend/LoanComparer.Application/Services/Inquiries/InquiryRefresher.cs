using LoanComparer.Application.Configuration;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;
using Microsoft.Extensions.Options;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class InquiryRefresher : IInquiryRefresher
{
    private readonly IReadOnlyList<IBankInterface> _bankInterfaces;
    private readonly IInquiryQuery _query;
    private readonly IInquiryCommand _command;
    private readonly IOptionsMonitor<InquiryConfiguration> _config;
    private readonly IEmailService _emailService;

    public InquiryRefresher(IBankInterfaceFactory bankInterfaceFactory, IInquiryQuery query, IInquiryCommand command,
        IOptionsMonitor<InquiryConfiguration> config, IEmailService emailService)
    {
        _bankInterfaces = bankInterfaceFactory.CreateBankInterfaces();
        _query = query;
        _command = command;
        _config = config;
        _emailService = emailService;
    }
    
    public async Task RefreshStatusesForUserAsync(string username)
    {
        var statusesToRefresh = await _query.GetPendingStatusesForUserAsync(username);
        foreach (var status in statusesToRefresh)
        {
            var refresher = GetBankInterfaceForStatus(status);
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
        var statusesToRefresh = await _query.GetAllStatusesThatShouldBeRefreshedAsync();
        foreach (var inquiryStatus in statusesToRefresh)
        {
            var bankInterface = GetBankInterfaceForStatus(inquiryStatus);
            var previousStatus = inquiryStatus.Status;
            var updated = await bankInterface.RefreshStatusAsync(inquiryStatus);
            if(updated.Status == previousStatus) continue;

            var inquiry = inquiryStatus.Inquiry;
            var email = new StatusChangedEmail(inquiry.PersonalData.NotificationEmail, inquiry.PersonalData.FirstName,
                string.Format(_config.CurrentValue.CheckInquiryStatusUrl, inquiry.Id));
            await _emailService.SendEmailAsync(email, CancellationToken.None);
        }
    }

    public async Task RefreshStatusesForInquiryAsync(Guid inquiryId)
    {
        var statusesToRefresh = await _query.GetStatusesForInquiryAsync(inquiryId);
        foreach (var status in statusesToRefresh)
        {
            var refresher = GetBankInterfaceForStatus(status);
            await refresher.RefreshStatusAsync(status);
        }
    }

    private IBankInterface GetBankInterfaceForStatus(SentInquiryStatus status)
    {
        var refresher = _bankInterfaces.FirstOrDefault(r => r.BankName == status.BankName);
        if (refresher is null)
            throw new InvalidOperationException(
                $"There is no known bank with name {status.BankName}, but status with id {status.Id} references it");
        return refresher;
    }
}