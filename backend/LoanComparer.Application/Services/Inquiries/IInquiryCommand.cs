using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryCommand
{
    /// <summary>
    ///     Saves <paramref name="status" /> into database. If corresponding inquiry is not yet saved, saves it as well.
    /// </summary>
    /// <param name="status"><see cref="SentInquiryStatus" /> to save</param>
    Task SaveInquiryStatusAsync(SentInquiryStatus status);

    Task<SentInquiryStatus> MarkAsRejectedAsync(SentInquiryStatus status);

    Task<SentInquiryStatus> MarkAsBankServerErrorAsync(SentInquiryStatus status);

    Task<SentInquiryStatus> MarkAsTimeoutAsync(SentInquiryStatus status);

    Task<SentInquiryStatus> LinkSavedOfferToStatusAsync(SentInquiryStatus status, Guid offerId);
}