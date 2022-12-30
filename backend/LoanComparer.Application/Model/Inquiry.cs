using LoanComparer.Application.DTO;

namespace LoanComparer.Application.Model;

public sealed class Inquiry
{
    public Guid Id { get; init; }

    public decimal AmountRequested { get; init; }

    public int NumberOfInstallments { get; init; }

    public PersonalData PersonalData { get; init; } = null!;

    public JobDetails JobDetails { get; init; } = null!;

    public GovernmentId GovernmentId { get; init; } = null!;

    public static Inquiry FromDto(InquiryDTO dto)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            AmountRequested = dto.AmountRequested,
            NumberOfInstallments = dto.NumberOfInstallments,
            PersonalData = PersonalData.FromDto(dto.PersonalData),
            JobDetails = JobDetails.FromDto(dto.JobDetails),
            GovernmentId = GovernmentId.FromDto(dto.GovtId)
        };
    }
}

public sealed class PersonalData
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public DateOnly BirthDate { get; init; }

    public static PersonalData FromDto(PersonalDataDTO dto)
    {
        return new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            BirthDate = dto.BirthDate
        };
    }
}

public sealed class JobDetails
{
    public string JobName { get; init; } = null!;

    public string? Description { get; init; } = null!;

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public static JobDetails FromDto(JobDetailsDTO dto)
    {
        return new()
        {
            JobName = dto.JobName,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
    }
}