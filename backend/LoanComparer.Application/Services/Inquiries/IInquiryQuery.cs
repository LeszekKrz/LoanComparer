using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryQuery
{
    Inquiry GetById(Guid id);

    SentInquiryStatus GetStatusById(Guid statusId);

    IReadOnlyList<SentInquiryStatus> GetPendingStatusesForUser(string username);

    IReadOnlyList<SentInquiryStatus> GetAllPendingStatuses();

    IReadOnlyList<SentInquiryStatus> GetPendingStatusesOlderThan(TimeSpan limit);

    // GetForUser()...
}