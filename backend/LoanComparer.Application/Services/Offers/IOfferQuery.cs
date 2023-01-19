using LoanComparer.Application.DTO.OfferApplicationDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;

namespace LoanComparer.Application.Services.Offers
{
    public interface IOfferQuery
    {
        public Task<OwnershipTestResult> CheckOwnerAsync(Guid offerId, string? username);

        public Task<SentInquiryStatus> GetStatusWithOfferOrThrowAsync(Guid offerId);

        public Task<IReadOnlyCollection<OfferApplicationDTO>> GetAllApplicationsForThisBank();

        public Task<byte[]> GetSignedDocument(Guid offerId);
    }
}
