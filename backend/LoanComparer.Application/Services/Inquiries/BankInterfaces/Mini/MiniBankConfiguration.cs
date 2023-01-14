using System.ComponentModel.DataAnnotations;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces.Mini;

public sealed class MiniBankConfiguration
{
    public const string SectionName = "BankApi:Mini";
    
    [Required]
    public string BaseUrl { get; init; } = null!;
    
    [Required]
    public string AuthUrl { get; init; } = null!;

    [Required] 
    public string AuthUsername { get; init; } = null!;
}