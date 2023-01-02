namespace LoanComparer.Application.DTO.InquiryDTO;

public sealed class SentInquiryStatusDTO
{
    public Guid BankId { get; init; }

    public string Status { get; init; } = null!;

    public Guid? OfferId { get; init; }
}