using System.ComponentModel.DataAnnotations;
using LoanComparer.Application.DTO.OfferDTO;

namespace LoanComparer.Application.Model;

public sealed class Offer
{
    public Guid Id { get; init; }
    
    public decimal LoanValue { get; init; }
    
    public int NumberOfInstallments { get; init; }
    
    public decimal Percentage { get; init; }
    
    public decimal MonthlyInstallment { get; init; }
    
    public OfferEntity ToEntity()
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

    public static Offer FromEntity(OfferEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            LoanValue = entity.LoanValue,
            NumberOfInstallments = entity.NumberOfInstallments,
            Percentage = entity.Percentage,
            MonthlyInstallment = entity.MonthlyInstallment
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
    public decimal LoanValue { get; init; }
    
    [Required]
    public int NumberOfInstallments { get; init; }
    
    [Required]
    public decimal Percentage { get; init; }
    
    [Required]
    public decimal MonthlyInstallment { get; init; }
}