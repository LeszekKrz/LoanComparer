namespace LoanComparer.Application.Model;

public sealed class SentInquiryStatus
{
    public Inquiry Inquiry { get; init; } = null!;

    public Guid BankId { get; init; }

    public InquiryStatus Status { get; init; }

    public Offer? ReceivedOffer { get; init; }
}

public enum InquiryStatus
{
    Pending,
    Accepted,
    Rejected,
    Timeout,
    Error
}