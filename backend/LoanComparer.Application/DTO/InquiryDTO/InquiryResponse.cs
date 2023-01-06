namespace LoanComparer.Application.DTO.InquiryDTO;

public sealed class InquiryResponse
{
    public Guid Id { get; init; }
    
    public string? OwnerUsername { get; init; }
    
    public DateTimeOffset CreationTime { get; init; }
    
    public decimal AmountRequested { get; init; }
    
    public int NumberOfInstallments { get; init; }
    
    public PersonalDataDTO PersonalData { get; init; } = null!;

    public JobDetailsDTO JobDetails { get; init; } = null!;

    public GovernmentIdDTO GovtId { get; init; } = null!;
}