using System.ComponentModel.DataAnnotations;

namespace LoanComparer.Application.Model;

public sealed class SentInquiryStatus
{
    public Guid Id { get; init; }
    
    public Inquiry Inquiry { get; init; } = null!;

    public Guid BankId { get; init; }

    public InquiryStatus Status { get; init; }

    public Offer? ReceivedOffer { get; init; }

    public SentInquiryStatusEntity ToEntity()
    {
        return new()
        {
            Id = Id,
            InquiryId = Inquiry.Id,
            BankId = BankId,
            Status = Status,
            OfferId = ReceivedOffer?.Id
        };
    }

    public static SentInquiryStatus FromEntity(SentInquiryStatusEntity e)
    {
        return new()
        {
            Id = e.Id,
            Inquiry = Inquiry.FromEntity(e.Inquiry),
            BankId = e.BankId,
            Status = e.Status,
            ReceivedOffer = e.Offer is not null ? Offer.FromEntity(e.Offer) : null,
        };
    }
}

public sealed class SentInquiryStatusEntity
{
    [Key]
    [Required]
    public Guid Id { get; init; }

    [Required] 
    public InquiryEntity Inquiry { get; init; } = null!;
    
    [Required]
    public Guid InquiryId { get; init; }
    
    [Required]
    public Guid BankId { get; init; }
    
    [Required]
    public InquiryStatus Status { get; set; }
    
    public OfferEntity? Offer { get; set; }
    
    public Guid? OfferId { get; set; }
}

public enum InquiryStatus
{
    Pending,
    Accepted,
    Rejected,
    Timeout,
    Error
}