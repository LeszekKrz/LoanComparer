using System.ComponentModel.DataAnnotations;

namespace LoanComparer.Application.DTO.InquiryDTO;

public sealed class InquiryRequest
{
    [Required]
    [Range(0, 10_000_000)]
    public decimal AmountRequested { get; init; }
    
    [Required]
    [Range(1, 12 * 100)]
    public int NumberOfInstallments { get; init; }
    
    [Required]
    public PersonalDataDTO PersonalData { get; init; } = null!;

    [Required]
    public JobDetailsDTO JobDetails { get; init; } = null!;

    [Required]
    public GovernmentIdDTO GovtId { get; init; } = null!;
}

public sealed class PersonalDataDTO
{
    [Required]
    public string FirstName { get; init; } = null!;

    [Required]
    public string LastName { get; init; } = null!;

    [Required]
    public DateOnly BirthDate { get; init; }
}

public sealed class JobDetailsDTO
{
    [Required]
    public string JobName { get; init; } = null!;

    public string? Description { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }
}