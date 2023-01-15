using LoanComparer.Application.DTO.OfferDTO;

namespace LoanComparer.Application.DTO.InquiryDTO;

public sealed class SentInquiryStatusDTO
{
    public string BankName { get; init; } = null!;

    public string Status { get; init; } = null!;

    public OfferResponse? Offer { get; init; }
}