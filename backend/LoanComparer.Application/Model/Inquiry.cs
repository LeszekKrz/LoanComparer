namespace LoanComparer.Application.Model;

public sealed class Inquiry
{
    public Guid Id { get; init; }

    public decimal AmountRequested { get; init; }

    public int NumberOfInstallments { get; init; }

    public PersonalData PersonalData { get; init; } = null!;

    public JobDetails JobDetails { get; init; } = null!;

    public GovernmentId GovernmentId { get; init; } = null!;
}

public sealed class PersonalData
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public DateOnly BirthDate { get; init; }
}

public sealed class JobDetails
{
    public string JobName { get; init; } = null!;

    public string? Description { get; init; } = null!;

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }
}