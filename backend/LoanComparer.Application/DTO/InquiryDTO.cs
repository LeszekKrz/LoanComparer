using LoanComparer.Application.Model;

namespace LoanComparer.Application.DTO;

public sealed class InquiryDTO
{
    public decimal AmountRequested { get; init; }
    
    public int NumberOfInstallments { get; init; }
    
    public PersonalDataDTO PersonalData { get; init; } = null!;

    public JobDetailsDTO JobDetails { get; init; } = null!;

    public GovernmentIdDTO GovtId { get; init; } = null!;
}

public sealed class PersonalDataDTO
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public DateOnly BirthDate { get; init; }
}

public sealed class JobDetailsDTO
{
    public string JobName { get; init; } = null!;

    public string? Description { get; init; } = null!;

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }
}