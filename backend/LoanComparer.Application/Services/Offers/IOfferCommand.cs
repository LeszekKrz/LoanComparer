using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Offers;

public interface IOfferCommand
{
    Task SaveOfferAsync(Offer offer);
}