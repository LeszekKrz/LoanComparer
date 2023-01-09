namespace LoanComparer.Application.DTO.InquiryDTO;

public sealed class InquiryRequest
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

    public string NotificationEmail { get; init; } = null!;
}

public sealed class JobDetailsDTO
{
    public string JobName { get; init; } = null!;
    
    public decimal IncomeLevel { get; init; }

    public string? Description { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }
}