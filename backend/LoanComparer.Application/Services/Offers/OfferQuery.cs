using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Services.Offers
{
    internal class OfferQuery : IOfferQuery
    {
        private readonly LoanComparerContext _context;
        private readonly IReadOnlyCollection<IBankInterface> _bankInterfaces;

        public OfferQuery(LoanComparerContext context, IBankInterfaceFactory bankInterfaceFactory)
        {
            _context = context;
            _bankInterfaces = bankInterfaceFactory.CreateBankInterfaces();
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

        public async Task<FileResult> GetDocumentAsync(Guid offerId)
        {
            OfferEntity? entity = await _context.Offers
                .Include(offer => offer.SentInquiryStatus)
                .SingleOrDefaultAsync(offer => offer.Id == offerId);
            if (entity is null)
                throw new InvalidOperationException($"There is no offer with id {offerId}");
            var bank = _bankInterfaces.SingleOrDefault(r => r.BankName == entity.SentInquiryStatus.BankName);
            if (bank is null)
                throw new InvalidOperationException(
                    $@"There is no known bank with name {entity.SentInquiryStatus.BankName},
                    but status with id {entity.SentInquiryStatus.Id} references it");
            return await bank.GetDocumentAsync(offerId);
        }
    }
}
