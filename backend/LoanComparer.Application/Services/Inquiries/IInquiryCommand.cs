using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IInquiryCommand
{
    /// <summary>
    ///     Saves <paramref name="status" /> into database. If corresponding inquiry is not yet saved, saves it as well.
    /// </summary>
    /// <param name="status"><see cref="SentInquiryStatus" /> to save</param>
    void SaveInquiryStatus(SentInquiryStatus status);

    SentInquiryStatus MarkAsRejected(SentInquiryStatus status);

    SentInquiryStatus MarkAsBankServerError(SentInquiryStatus status);

    SentInquiryStatus MarkAsTimeout(SentInquiryStatus status);

    SentInquiryStatus LinkSavedOfferToInquiry(SentInquiryStatus status, Guid offerId);
}