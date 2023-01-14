using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;

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

    public async Task SetStatusOfAnOfferAsync(Guid offerId, InquiryStatus inquiryStatus)
    {
        OfferEntity entity = await GetOfferEntityWithStatusOrThrow(offerId);
        entity.SentInquiryStatus.Status = inquiryStatus;
        await _context.SaveChangesAsync();
    }

    private async Task<OfferEntity> GetOfferEntityWithStatusOrThrow(Guid offerId)
    {
        OfferEntity? entity = await _context.Offers
                .Include(offer => offer.SentInquiryStatus)
                .SingleOrDefaultAsync(offer => offer.Id == offerId);
        if (entity is null)
            throw new InvalidOperationException($"There is no offer with id {offerId}");
        return entity;
    }
}