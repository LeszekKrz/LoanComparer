using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Services.Offers;

public sealed class OfferCommand : IOfferCommand
{
    private readonly LoanComparerContext _context;
    private readonly IReadOnlyCollection<IBankInterface> _banks;

    public OfferCommand(LoanComparerContext context, IBankInterfaceFactory bankInterfaceFactory)
    {
        _context = context;
        _banks = bankInterfaceFactory.CreateBankInterfaces();
    }

    public async Task<SentInquiryStatus> ApplyForAnOfferAsync(Guid offerId, IFormFile file)
    {
        OfferEntity? entity = await _context.Offers
                .Include(offer => offer.SentInquiryStatus)
                .SingleOrDefaultAsync(offer => offer.Id == offerId);
        if (entity is null)
            throw new InvalidOperationException($"There is no offer with id {offerId}");
        var bank = _banks.SingleOrDefault(r => r.BankName == entity.SentInquiryStatus.BankName);
        if (bank is null)
            throw new InvalidOperationException(
                $@"There is no known bank with name {entity.SentInquiryStatus.BankName},
                    but status with id {entity.SentInquiryStatus.Id} references it");
        return await bank.ApplyForAnOfferAsync(offerId, file);
    }

    public async Task SaveOfferAsync(Offer offer)
    {
        _context.Add(offer.ToEntity());
        await _context.SaveChangesAsync();
    }
}