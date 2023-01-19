using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.OfferApplicationDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
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

        public async Task<IReadOnlyCollection<OfferApplicationDTO>> GetAllApplicationsForThisBank()
        {
            var applicationStatuses = new InquiryStatus[]
            {
                InquiryStatus.WaitingForAcceptance,
                InquiryStatus.Accepted,
                InquiryStatus.Rejected
            };

            return await _context.InquiryStatuses
                .Include(inquiryStatus => inquiryStatus.Offer)
                .Include(inquiryStatus => inquiryStatus.Inquiry)
                .Where(inquiryStatus => applicationStatuses.Contains(inquiryStatus.Status)
                    && inquiryStatus.BankName == LoanComparerConstants.OurBankName)
                .Select(inquiryStatus => new OfferApplicationDTO(
                    (Guid)inquiryStatus.OfferId!,
                    inquiryStatus.Offer!.LoanValueAsSmallestNominal,
                    inquiryStatus.Offer.NumberOfInstallments,
                    inquiryStatus.Offer.Percentage,
                    inquiryStatus.Offer.MonthlyInstallmentAsSmallestNominal,
                    inquiryStatus.Status,
                    inquiryStatus.Inquiry.CreationTimestamp,
                    (long)inquiryStatus.Offer.DateOfApplication!,
                    inquiryStatus.Inquiry.NotificationEmail,
                    inquiryStatus.Inquiry.GovernmentIdType,
                    inquiryStatus.Inquiry.GovernmentIdValue
                ))
                .ToListAsync();
        }

        public async Task<byte[]> GetSignedDocument(Guid offerId)
        {
            OfferEntity? offer = await _context.Offers.SingleOrDefaultAsync(offer => offer.Id == offerId);
            if (offer == null)
                throw new InvalidOperationException($"There is no offer with id {offerId}");
            if (offer.SignedContractContent == null)
                throw new InvalidOperationException($"Signed contract for offer with id {offerId} is null");

            return offer.SignedContractContent;
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
