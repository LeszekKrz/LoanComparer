using System.ComponentModel.DataAnnotations;
using LoanComparer.Application.DTO.InquiryDTO;

namespace LoanComparer.Application.Model;

public sealed class SentInquiryStatus
{
    public Guid Id { get; init; }
    
    public Inquiry Inquiry { get; init; } = null!;

    public string BankName { get; init; } = null!;

    public InquiryStatus Status { get; init; }

    public Offer? ReceivedOffer { get; init; }

    public SentInquiryStatusEntity ToEntity()
    {
        return new()
        {
            Id = Id,
            InquiryId = Inquiry.Id,
            BankName = BankName,
            Status = Status,
            OfferId = ReceivedOffer?.Id
        };
    }

    public SentInquiryStatusDTO ToDto()
    {
        return new()
        {
            BankName = BankName,
            OfferId = ReceivedOffer?.Id,
            Status = Status switch
            {
                InquiryStatus.Pending => "PENDING",
                InquiryStatus.Accepted => "OFFERRECEIVED",
                InquiryStatus.Rejected => "REJECTED",
                InquiryStatus.Timeout => "TIMEOUT",
                InquiryStatus.Error => "ERROR",
                _ => throw new ArgumentOutOfRangeException()
            }
        };
    }

    public static SentInquiryStatus FromEntity(SentInquiryStatusEntity e)
    {
        return new()
        {
            Id = e.Id,
            Inquiry = Inquiry.FromEntity(e.Inquiry),
            BankName = e.BankName,
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
    public string BankName { get; init; } = null!;
    
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