using System.ComponentModel.DataAnnotations;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces.This;

public sealed class ThisBankConfiguration
{
    public const string SectionName = "BankApi:This";
    
    [Required]
    public string BaseUrl { get; init; } = null!;

    [Required] 
    public string AuthUsername { get; init; } = null!;
}