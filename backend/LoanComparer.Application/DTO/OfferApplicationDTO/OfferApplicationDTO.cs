using EllipticCurve;
using LoanComparer.Application.Model;

namespace LoanComparer.Application.DTO.OfferApplicationDTO
{
    public sealed record OfferApplicationDTO(
        Guid OfferId,
        decimal LoanValue,
        int NumberOfInstallments,
        double Percentage,
        decimal MonthlyInstallment,
        string Status,
        DateTimeOffset DateOfInquiry,
        DateTimeOffset DateOfApplication,
        string Email,
        GovernmentIdDTO GovernmentId)
    {
        public OfferApplicationDTO(
            Guid offerId,
            decimal loanValueAsSmallestNominal,
            int numberOfInstallments,
            double percentage,
            decimal monthlyInstallments,
            InquiryStatus inquiryStatus,
            long dateOfInquiry,
            long dateOfApplication,
            string email,
            string governmentIdType,
            string governmentIdValue) : this(
                offerId,
                loanValueAsSmallestNominal / 100m,
                numberOfInstallments,
                percentage,
                monthlyInstallments,
                inquiryStatus switch
                {
                    InquiryStatus.WaitingForAcceptance => "WAITINGFORACCEPTANCE",
                    InquiryStatus.Accepted => "ACCEPTED",
                    InquiryStatus.Rejected => "REJECTED",
                    _ => throw new ArgumentOutOfRangeException()
                },
                DateTimeOffset.FromUnixTimeMilliseconds(dateOfInquiry),
                DateTimeOffset.FromUnixTimeMilliseconds(dateOfApplication),
                email,
                new GovernmentIdDTO(governmentIdType, governmentIdValue))
        { }
    }
}
