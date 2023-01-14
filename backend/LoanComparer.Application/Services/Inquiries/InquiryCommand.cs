using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class InquiryCommand : IInquiryCommand
{
    private readonly LoanComparerContext _context;
    
    public InquiryCommand(LoanComparerContext context)
    {
        _context = context;
    }
    
    public async Task SaveInquiryStatusAsync(SentInquiryStatus status)
    {
        await SaveInquiryIfNotYetAddedAsync(status.Inquiry);
        if (status.ReceivedOffer is not null) await SaveOfferAsync(status.ReceivedOffer);
        
        var entity = status.ToEntity();
        _context.InquiryStatuses.Add(entity);
        await _context.SaveChangesAsync();
    }

    private async Task SaveInquiryIfNotYetAddedAsync(Inquiry inquiry)
    {
        if (await _context.Inquiries.FirstOrDefaultAsync(i => i.Id == inquiry.Id) is not null) return;
        _context.Add(inquiry.ToEntity());
        await _context.SaveChangesAsync();
    }

    private async Task SaveOfferAsync(Offer offer)
    {
        _context.Offers.Add(offer.ToEntity());
        await _context.SaveChangesAsync();
    }

    public async Task<SentInquiryStatus> MarkAsRejectedAsync(SentInquiryStatus status)
    {
        var entity = await GetEntityAndThrowIfNotPresent(status);
        entity.Status = InquiryStatus.Rejected;
        await _context.SaveChangesAsync();
        return SentInquiryStatus.FromEntity(entity);
    }

    public async Task<SentInquiryStatus> MarkAsBankServerErrorAsync(SentInquiryStatus status)
    {
        var entity = await GetEntityAndThrowIfNotPresent(status);
        entity.Status = InquiryStatus.Error;
        await _context.SaveChangesAsync();
        return SentInquiryStatus.FromEntity(entity);
    }

    public async Task<SentInquiryStatus> MarkAsTimeoutAsync(SentInquiryStatus status)
    {
        var entity = await GetEntityAndThrowIfNotPresent(status);
        entity.Status = InquiryStatus.Timeout;
        await _context.SaveChangesAsync();
        return SentInquiryStatus.FromEntity(entity);
    }

    public async Task<SentInquiryStatus> LinkSavedOfferToStatusAsync(SentInquiryStatus status, Guid offerId)
    {
        var entity = await GetEntityAndThrowIfNotPresent(status);
        entity.Status = InquiryStatus.OfferReceived;
        entity.OfferId = offerId;
        await _context.SaveChangesAsync();
        return SentInquiryStatus.FromEntity(await GetEntityAndThrowIfNotPresent(status));
    }

    private async Task<SentInquiryStatusEntity> GetEntityAndThrowIfNotPresent(SentInquiryStatus status)
    {
        var entity = await _context.InquiryStatuses.
            Include(s =>s.Inquiry).
            Include(s => s.Offer).
            FirstOrDefaultAsync(s => s.Id == status.Id);
        if (entity is null)
            throw new InvalidOperationException(
                $"Attempted to modify a status with id {status.Id}, but it is not present in the database");
        return entity;
    }
}