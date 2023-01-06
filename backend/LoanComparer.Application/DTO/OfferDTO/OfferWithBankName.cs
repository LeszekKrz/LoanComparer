using LoanComparer.Application.Model;

namespace LoanComparer.Application.DTO.OfferDTO;

public sealed class OfferWithBankName
{
    public string BankName { get; init; } = null!;

    public OfferResponse Offer { get; init; } = null!;

    public static OfferWithBankName? FromSentInquiryStatus(SentInquiryStatus status)
    {
        if (status.ReceivedOffer is null) return null;
        return new()
        {
            BankName = status.BankName,
            Offer = status.ReceivedOffer.ToDto()
        };
    }
}