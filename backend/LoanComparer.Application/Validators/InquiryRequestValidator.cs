using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.InquiryDTO;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Validators;

public sealed class InquiryRequestValidator : AbstractValidator<InquiryRequest>
{
    public InquiryRequestValidator(LoanComparerContext context)
    {
        RuleFor(i => i.AmountRequested).GreaterThan(0).LessThan(10_000_000);
        RuleFor(i => i.NumberOfInstallments).GreaterThan(0).LessThan(12 * 100);
        RuleFor(i => i.JobDetails).NotNull();
        RuleFor(i => i.GovernmentId).NotNull();
        RuleFor(i => i.PersonalData).NotNull();

        RuleFor(i => i.GovernmentId.Type).
            NotEmpty().
            Must(type => LoanComparerConstants.GovernmentIdTypes.Contains(type)).
            WithMessage("Invalid government id type");
        
        When(x => x.GovernmentId.Type == "PESEL", () =>
        {
            RuleFor(x => x.GovernmentId.Value).
                Length(LoanComparerConstants.PeselLength).
                Matches(@"^\d{11}$");
        });

        When(x => x.GovernmentId.Type == "ID Number", () =>
        {
            RuleFor(x => x.GovernmentId.Value).
                Length(LoanComparerConstants.IdNumberLength).
                Matches(@"^[a-zA-Z]{3}\d{6}$");
        });

        When(x => x.GovernmentId.Type == "Passport Number", () =>
        {
            RuleFor(x => x.GovernmentId.Value).
                Length(LoanComparerConstants.PassportNumberLength).
                Matches(@"^[a-zA-Z]{2}\d{7}$");
        });

        RuleFor(x => x.JobDetails.IncomeLevel).GreaterThan(0);
        RuleFor(x => x.JobDetails.JobName).
            MustAsync(async (jobType, cancellationToken) =>
                await context.JobTypes.AnyAsync(jt => jt.Name == jobType, cancellationToken)).
            WithMessage(x => $"No job type with name {x.JobDetails.JobName} found");

        RuleFor(i => i.PersonalData.FirstName).NotEmpty();
        RuleFor(i => i.PersonalData.LastName).NotEmpty();
        RuleFor(i => i.PersonalData.NotificationEmail).NotEmpty().EmailAddress();
        RuleFor(i => i.PersonalData.BirthDate).
            LessThan(DateOnly.FromDateTime(DateTime.Now)).
            GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-150)));
    }
}