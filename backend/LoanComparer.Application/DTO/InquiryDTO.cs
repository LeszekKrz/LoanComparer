using LoanComparer.Application.Model;

namespace LoanComparer.Application.DTO;

public sealed class InquiryDTO
{
    public decimal AmountRequested { get; init; }
    
    public int NumberOfInstallments { get; init; }
    
    public PersonalDataDTO PersonalData { get; init; } = null!;

    public JobDetailsDTO JobDetails { get; init; } = null!;

    public GovernmentIdDTO GovernmentId { get; init; } = null!;

    public Inquiry ToInquiry()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            AmountRequested = AmountRequested,
            NumberOfInstallments = NumberOfInstallments,
            PersonalData = PersonalData.ToPersonalData(),
            JobDetails = JobDetails.ToJobDetails(),
            GovernmentId = GovernmentId.ToGovernmentId()
            // TODO: Change these ^^^ to static FromRequest() in model classes
        };
    }
}

public sealed class PersonalDataDTO
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public DateOnly BirthDate { get; init; }

    public PersonalData ToPersonalData()
    {
        return new()
        {
            FirstName = FirstName,
            LastName = LastName,
            BirthDate = BirthDate
        };
    }
}

public sealed class JobDetailsDTO
{
    public string JobName { get; init; } = null!;

    public string? Description { get; init; } = null!;

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public JobDetails ToJobDetails()
    {
        return new()
        {
            JobName = JobName,
            Description = Description,
            StartDate = StartDate,
            EndDate = EndDate
        };
    }
}