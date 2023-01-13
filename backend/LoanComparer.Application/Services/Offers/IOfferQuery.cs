using LoanComparer.Application.Services.Inquiries;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Application.Services.Offers
{
    public interface IOfferQuery
    {
        public Task<OwnershipTestResult> CheckOwnerAsync(Guid offerId, string? username);

        public Task<FileResult> GetDocumentAsync(Guid offerId);
    }
}
