using LoanComparer.Application.DTO.OfferDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;
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

    public async Task<InquiryStatus> ApplyForAnOfferAsync(Guid offerId, IFormFile file)
    {
        OfferEntity entity = await GetOfferEntityWithStatusOrThrow(offerId);
        var bank = _banks.SingleOrDefault(r => r.BankName == entity.SentInquiryStatus.BankName);
        if (bank is null)
            throw new InvalidOperationException(
                $@"There is no known bank with name {entity.SentInquiryStatus.BankName},
                    but status with id {entity.SentInquiryStatus.Id} references it");
        return await bank.ApplyForAnOfferAsync(entity, file);
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