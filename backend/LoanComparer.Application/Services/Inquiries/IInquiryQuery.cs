using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryQuery
{
    Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesForUserAsync(string username);

    Task<IReadOnlyList<SentInquiryStatus>> GetAllStatusesThatShouldBeRefreshedAsync();

    Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesOlderThanAsync(TimeSpan limit);

    Task<IReadOnlyList<Inquiry>> GetAllForUserAsync(string username);

    Task<IReadOnlyList<SentInquiryStatus>> GetStatusesForInquiryAsync(Guid inquiryId);

    Task<OwnershipTestResult> CheckOwnerAsync(Guid inquiryId, string? username);

    Task<IReadOnlyCollection<InquiryResponse>> GetAllInquiries();
}

public enum OwnershipTestResult
{
    Allowed,
    Unauthorized,
    DoesNotExist
}