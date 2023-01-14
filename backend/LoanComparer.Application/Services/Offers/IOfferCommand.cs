using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Offers;

public interface IOfferCommand
{
    Task SaveOfferAsync(Offer offer);

    Task SetStatusOfAnOfferAsync(Guid offerId, InquiryStatus inquiryStatus);
}