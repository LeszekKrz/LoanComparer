using System.ComponentModel.DataAnnotations;
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

    public InquiryEntity ToEntity()
    {
        return new()
        {
            Id = Id,
            AmountRequested = AmountRequested,
            NumberOfInstallments = NumberOfInstallments,
            FirstName = PersonalData.FirstName,
            LastName = PersonalData.LastName,
            BirthDate = PersonalData.BirthDate,
            JobName = JobDetails.JobName,
            JobDescription = JobDetails.Description,
            JobStartDate = JobDetails.StartDate,
            JobEndDate = JobDetails.EndDate,
            GovtIdType = GovernmentId.Type,
            GovtIdValue = GovernmentId.Value
        };
    }

    public static Inquiry FromEntity(InquiryEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            AmountRequested = entity.AmountRequested,
            NumberOfInstallments = entity.NumberOfInstallments,
            PersonalData = new()
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate
            },
            JobDetails = new()
            {
                JobName = entity.JobName,
                Description = entity.JobDescription,
                StartDate = entity.JobStartDate,
                EndDate = entity.JobEndDate
            },
            GovernmentId = new(entity.GovtIdType, entity.GovtIdValue)
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

public sealed class InquiryEntity
{
    // TODO: Move validators to separate class
    
    [Key]
    [Required]
    public Guid Id { get; init; }
    
    [Required]
    public decimal AmountRequested { get; init; }

    [Required]
    public int NumberOfInstallments { get; init; }

    [Required]
    public string FirstName { get; init; } = null!;

    [Required]
    public string LastName { get; init; } = null!;

    [Required]
    public DateOnly BirthDate { get; init; }

    [Required]
    public string JobName { get; init; } = null!;
    
    public string? JobDescription { get; init; } = null!;
    
    public DateOnly? JobStartDate { get; init; }

    public DateOnly? JobEndDate { get; init; }

    [Required] 
    public string GovtIdType { get; init; } = null!;

    [Required] 
    public string GovtIdValue { get; init; } = null!;
}