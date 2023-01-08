namespace LoanComparer.Application.DTO.OfferDTO;

public sealed class OfferResponse
{
    public Guid Id { get; init; }
    
    public decimal LoanValue { get; init; }
    
    public int NumberOfInstallments { get; init; }
    
    public double Percentage { get; init; }
    
    public decimal MonthlyInstallment { get; init; }
}