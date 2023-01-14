using LoanComparer.Application.Services.Inquiries;

namespace LoanComparer.Application.Services.Offers
{
    public interface IOfferQuery
    {
        public Task<OwnershipTestResult> CheckOwnerAsync(Guid offerId, string? username);

        public Task<byte[]> GetDocumentContentAsync(Guid offerId);
    }
}
