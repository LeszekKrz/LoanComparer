using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryQuery
{
    Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesForUserAsync(string username);

    Task<IReadOnlyList<SentInquiryStatus>> GetAllPendingStatusesAsync();

    Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesOlderThanAsync(TimeSpan limit);

    Task<IReadOnlyList<Inquiry>> GetAllAsync();

    Task<IReadOnlyList<SentInquiryStatus>> GetStatusesForInquiryAsync(Guid inquiryId);
}