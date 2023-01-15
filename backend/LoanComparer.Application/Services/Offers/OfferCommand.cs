using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Offers;

public sealed class OfferCommand : IOfferCommand
{
    private readonly LoanComparerContext _context;

    public OfferCommand(LoanComparerContext context)
    {
        _context = context;
    }
    
    public async Task SaveOfferAsync(Offer offer)
    {
        _context.Add(offer.ToEntity());
        await _context.SaveChangesAsync();
    }
}