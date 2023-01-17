using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Services.Offers
{
    public class OfferQuery : IOfferQuery
    {
        private readonly LoanComparerContext _context;

        public OfferQuery(LoanComparerContext context)
        {
            _context = context;
        }

        public async Task<OwnershipTestResult> CheckOwnerAsync(Guid offerId, string? username)
        {
            OfferEntity? entity = await _context.Offers
                .Include(offer => offer.SentInquiryStatus)
                .ThenInclude(sentInquiryStatus => sentInquiryStatus.Inquiry)
                .SingleOrDefaultAsync(offer => offer.Id == offerId);
            if (entity is null) return OwnershipTestResult.DoesNotExist;

            return entity.SentInquiryStatus.Inquiry.OwnerUsername is null || entity.SentInquiryStatus.Inquiry.OwnerUsername == username
                ? OwnershipTestResult.Allowed
                : OwnershipTestResult.Unauthorized;
        }

        public async Task<SentInquiryStatus> GetStatusWithOfferOrThrowAsync(Guid offerId)
        {
            SentInquiryStatusEntity? sentInquiryStatusEntity = await _context.InquiryStatuses
                .Include(inquiryStatus => inquiryStatus.Offer)
                .SingleOrDefaultAsync(inquiryStatus => inquiryStatus.OfferId == offerId);
            if (sentInquiryStatusEntity is null || sentInquiryStatusEntity.Offer is null)
                throw new InvalidOperationException($"There is no offer with id {offerId}");
            return SentInquiryStatus.FromEntity(sentInquiryStatusEntity);
        }
    }
}
