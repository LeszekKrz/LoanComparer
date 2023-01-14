using LoanComparer.Application.DTO.OfferDTO;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces;

public interface IBankInterface
{
    string BankName { get; }

    /// <summary>
    ///     Refreshes <paramref name="status" /> by querying a remote API and updating data in database. If an offer is
    ///     received, saves it as well, and links it to <paramref name="status" />.
    /// </summary>
    /// <param name="status"><see cref="SentInquiryStatus" /> to update</param>
    /// <returns>Updated object</returns>
    Task<SentInquiryStatus> RefreshStatusAsync(SentInquiryStatus status);
    
    Task<SentInquiryStatus> SendInquiryAsync(Inquiry inquiry);

    Task<byte[]> GetDocumentContentAsync(OfferEntity offerEntity);

    Task<InquiryStatus> ApplyForAnOfferAsync(OfferEntity offerEntity, IFormFile file);
}