using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Offers;

public interface IOfferCommand
{
    Task SaveOfferAsync(Offer offer);

    Task UpdateStatusAndApplicationRelatedFieldsAsync(Guid offerId, InquiryStatus inquiryStatus, IFormFile formFile);

    Task UpdateStatusAsync(Guid offerId, InquiryStatus inquiryStatus);
}