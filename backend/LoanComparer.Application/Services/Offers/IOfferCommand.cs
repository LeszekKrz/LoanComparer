using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Offers;

public interface IOfferCommand
{
    Task SaveOfferAsync(Offer offer);

    Task UpdateStatusAndAddSignedContractAsync(Guid offerId, InquiryStatus inquiryStatus, IFormFile formFile);
}