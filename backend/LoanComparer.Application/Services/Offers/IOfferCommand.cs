using LoanComparer.Application.DTO.OfferDTO;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Offers;

public interface IOfferCommand
{
    Task SaveOfferAsync(Offer offer);

    Task<InquiryStatus> ApplyForAnOfferAsync(Guid offerid, IFormFile file);

    Task SetStatusOfAnOfferAsync(Guid offerId, InquiryStatus inquiryStatus);
}