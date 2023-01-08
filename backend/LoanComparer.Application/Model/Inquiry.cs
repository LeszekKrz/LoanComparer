using System.ComponentModel.DataAnnotations;
using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.Exceptions;

namespace LoanComparer.Application.Model;

public sealed class Inquiry
{
    public Guid Id { get; init; }
    
    public string? OwnerUsername { get; init; }

    public string NotificationEmail { get; init; } = null!;
    
    public DateTimeOffset CreationTime { get; init; }

    public decimal AmountRequested { get; init; }

    public int NumberOfInstallments { get; init; }

    public PersonalData PersonalData { get; init; } = null!;

    public JobDetails JobDetails { get; init; } = null!;

    public GovernmentId GovernmentId { get; init; } = null!;

    public static Inquiry FromRequest(InquiryRequest request, string? ownerUsername)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            OwnerUsername = ownerUsername,
            NotificationEmail = request.NotificationEmail ?? ownerUsername ?? throw new BadRequestException(new[]
                { new ErrorResponseDTO("Notification email is required for anonymous inquiries") }),
            AmountRequested = request.AmountRequested,
            NumberOfInstallments = request.NumberOfInstallments,
            CreationTime = DateTimeOffset.Now,
            PersonalData = PersonalData.FromDto(request.PersonalData),
            JobDetails = JobDetails.FromDto(request.JobDetails),
            GovernmentId = GovernmentId.FromDto(request.GovtId)
        };
    }

    public InquiryResponse ToResponse()
    {
        return new()
        {
            Id = Id,
            OwnerUsername = OwnerUsername,
            CreationTime = CreationTime,
            AmountRequested = AmountRequested,
            NumberOfInstallments = NumberOfInstallments,
            PersonalData = PersonalData.ToDto(),
            JobDetails = JobDetails.ToDto(),
            GovtId = GovernmentId.ToDto()
        };
    }

    public InquiryEntity ToEntity()
    {
        return new()
        {
            Id = Id,
            OwnerUsername = OwnerUsername,
            NotificationEmail = NotificationEmail,
            CreationTimestamp = CreationTime.ToUnixTimeMilliseconds(),
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
            OwnerUsername = entity.OwnerUsername,
            NotificationEmail = entity.NotificationEmail,
            CreationTime = DateTimeOffset.FromUnixTimeMilliseconds(entity.CreationTimestamp),
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

    public PersonalDataDTO ToDto()
    {
        return new()
        {
            FirstName = FirstName,
            LastName = LastName,
            BirthDate = BirthDate
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

    public JobDetailsDTO ToDto()
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

public sealed class InquiryEntity
{
    // TODO: Move validators to separate class
    
    [Key]
    [Required]
    public Guid Id { get; init; }
    
    public string? OwnerUsername { get; init; }
    
    [Required]
    public string NotificationEmail { get; init; } = null!;
    
    [Required]
    public long CreationTimestamp { get; init; }
    
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
    
    public string? JobDescription { get; init; }
    
    public DateOnly? JobStartDate { get; init; }

    public DateOnly? JobEndDate { get; init; }

    [Required] 
    public string GovtIdType { get; init; } = null!;

    [Required] 
    public string GovtIdValue { get; init; } = null!;
}