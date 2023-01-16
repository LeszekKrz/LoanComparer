using System.ComponentModel.DataAnnotations;
using LoanComparer.Application.DTO.OfferDTO;

namespace LoanComparer.Application.Model;

public sealed class Offer
{
    public Guid Id { get; init; }
    
    public decimal LoanValue { get; init; }
    
    public int NumberOfInstallments { get; init; }
    
    public double Percentage { get; init; }
    
    public decimal MonthlyInstallment { get; init; }
    
    public string DocumentLink { get; init; } = null!;
    
    public OfferEntity ToEntity()
    {
        return new()
        {
            Id = Id,
            LoanValueAsSmallestNominal = (long)(LoanValue * 100),
            NumberOfInstallments = NumberOfInstallments,
            Percentage = Percentage,
            MonthlyInstallmentAsSmallestNominal = (long)(MonthlyInstallment * 100),
            DocumentLink = DocumentLink,
        };
    }

    public static Offer FromEntity(OfferEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            LoanValue = entity.LoanValueAsSmallestNominal / 100m,
            NumberOfInstallments = entity.NumberOfInstallments,
            Percentage = entity.Percentage,
            MonthlyInstallment = entity.MonthlyInstallmentAsSmallestNominal / 100m,
            DocumentLink = entity.DocumentLink,
        };
    }

    public OfferResponse ToDto()
    {
        return new()
        {
            Id = Id,
            LoanValue = LoanValue,
            NumberOfInstallments = NumberOfInstallments,
            Percentage = Percentage,
            MonthlyInstallment = MonthlyInstallment
        };
    }
}

public sealed class OfferEntity
{
    [Key]
    [Required]
    public Guid Id { get; init; }
    
    [Required]
    public long LoanValueAsSmallestNominal { get; init; }
    
    [Required]
    public int NumberOfInstallments { get; init; }
    
    [Required]
    public double Percentage { get; init; }
    
    [Required]
    public long MonthlyInstallmentAsSmallestNominal { get; init; }

    [Required]
    public string DocumentLink { get; init; } = null!;

    public SentInquiryStatusEntity SentInquiryStatus { get; init; } = null!;

    public byte[]? SignedContractContent { get; set; }
}