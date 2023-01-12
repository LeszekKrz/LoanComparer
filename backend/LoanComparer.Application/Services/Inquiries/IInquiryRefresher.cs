namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryRefresher
{
    Task RefreshStatusesForUserAsync(string username);

    Task MarkOldStatusesAsTimeoutAsync();

    Task RefreshAllStatusesAndSendNotificationsAsync();

    Task RefreshStatusesForInquiryAsync(Guid inquiryId);
}