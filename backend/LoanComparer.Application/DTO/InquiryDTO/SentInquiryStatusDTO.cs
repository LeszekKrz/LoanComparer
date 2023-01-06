namespace LoanComparer.Application.DTO.InquiryDTO;

public sealed class SentInquiryStatusDTO
{
    public string BankName { get; init; } = null!;

    public string Status { get; init; } = null!;

    public Guid? OfferId { get; init; }
}