namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryRefresher
{
    void RefreshStatusesForUser(string username);

    void MarkOldStatusesAsTimeout();

    void RefreshAllStatusesAndSendNotifications();
}