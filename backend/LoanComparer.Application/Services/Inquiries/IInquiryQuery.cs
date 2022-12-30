using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryQuery
{
    Task<Inquiry> GetByIdAsync(Guid id);

    Task<SentInquiryStatus> GetStatusByIdAsync(Guid statusId);

    Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesForUserAsync(string username);

    Task<IReadOnlyList<SentInquiryStatus>> GetAllPendingStatusesAsync();

    Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesOlderThanAsync(TimeSpan limit);

    // GetForUser()...
}