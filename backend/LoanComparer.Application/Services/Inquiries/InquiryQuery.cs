using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class InquiryQuery : IInquiryQuery
{
    private readonly LoanComparerContext _context;

    public InquiryQuery(LoanComparerContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesForUserAsync(string username)
    {
        return await _context.InquiryStatuses.Include(s => s.Inquiry).
            Include(s => s.Offer).
            Where(s => s.Inquiry.OwnerUsername == username).
            Where(s => s.Status == InquiryStatus.Pending).
            Select(s => SentInquiryStatus.FromEntity(s)).
            ToListAsync();
    }

    public async Task<IReadOnlyList<SentInquiryStatus>> GetAllStatusesThatShouldBeRefreshedAsync()
    {
        return await _context.InquiryStatuses.Include(s => s.Inquiry).
            Include(s => s.Offer).
            Where(s => s.Status == InquiryStatus.Pending || s.Status == InquiryStatus.WaitingForAcceptance).
            Select(s => SentInquiryStatus.FromEntity(s)).
            ToListAsync();
    }

    public async Task<IReadOnlyList<SentInquiryStatus>> GetPendingStatusesOlderThanAsync(TimeSpan limit)
    {
        var oldest = DateTimeOffset.Now - limit;
        return await _context.InquiryStatuses.Include(s => s.Inquiry).
            Include(s => s.Offer).
            Where(s => s.Inquiry.CreationTimestamp < oldest.ToUnixTimeMilliseconds()).
            Where(s => s.Status == InquiryStatus.Pending).
            Select(s => SentInquiryStatus.FromEntity(s)).
            ToListAsync();
    }

    public async Task<IReadOnlyList<Inquiry>> GetAllForUserAsync(string username)
    {
        return await _context.Inquiries.Where(i => i.OwnerUsername == username).
            Select(i => Inquiry.FromEntity(i)).
            ToListAsync();
    }

    public async Task<IReadOnlyList<SentInquiryStatus>> GetStatusesForInquiryAsync(Guid inquiryId)
    {
        return await _context.InquiryStatuses.Include(s =>s.Inquiry).
            Include(s => s.Offer).
            Where(s => s.InquiryId == inquiryId).
            Select(s => SentInquiryStatus.FromEntity(s)).
            ToListAsync();
    }

    public async Task<OwnershipTestResult> CheckOwnerAsync(Guid inquiryId, string? username)
    {
        var entity = await _context.Inquiries.FirstOrDefaultAsync(i => i.Id == inquiryId);
        if (entity is null) return OwnershipTestResult.DoesNotExist;

        return entity.OwnerUsername is null || entity.OwnerUsername == username
            ? OwnershipTestResult.Allowed
            : OwnershipTestResult.Unauthorized;
    }

    public async Task<IReadOnlyCollection<InquiryResponse>> GetAllInquiries()
    {
        return (await _context.Inquiries
            .Select(inquiryEntity => Inquiry.FromEntity(inquiryEntity).ToResponse())
            .ToListAsync())
            .OrderByDescending(inquiryResponse => inquiryResponse.CreationTime).ToList();
    }
}