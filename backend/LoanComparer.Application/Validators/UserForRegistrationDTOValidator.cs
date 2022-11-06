using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.UserDTO;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Validators
{
    public class UserForRegistrationDTOValidator : AbstractValidator<UserForRegistrationDTO>
    {
        public UserForRegistrationDTOValidator(LoanComparerContext context)
        {
            RuleFor(x => x.FirstName).NotNull().Length(1, LoanComparerConstants.MaxFirstNameLength);

            RuleFor(x => x.LastName).NotNull().Length(1, LoanComparerConstants.MaxLastNameLength);

            RuleFor(x => x.Email).NotNull().Length(1, LoanComparerConstants.MaxEmailLength).EmailAddress();

            RuleFor(x => x.JobType)
                .MustAsync(async (jobType, cancellationToken) => await context.JobTypes.AnyAsync(jt => jt.Name == jobType.Name, cancellationToken))
                .WithMessage(x => $"No job type with name {x.JobType.Name} found");

            RuleFor(x => x.IncomeLevel).NotNull().GreaterThan(0);

            RuleFor(x => x.GovernmentIdType).NotNull().NotEmpty()
                .Must(governmentIdType => LoanComparerConstants.GovernmentIdTypes.Contains(governmentIdType))
                .WithMessage("Invalid government id type");

            RuleFor(x => x.GovernmentIdValue).NotNull().NotEmpty();

            When(x => x.GovernmentIdType == "PESEL", () =>
            {
                RuleFor(x => x.GovernmentIdValue).Length(LoanComparerConstants.PeselLength).Matches(@"^\d{11}$"); // 11 jako argument jak?
            }); // jest jeszcze bardziej skomplikowana logika na pesel jakies tam dodawanie tych liczb ale to moze sie pozniej doda

            When(x => x.GovernmentIdType == "ID Number", () =>
            {
                RuleFor(x => x.GovernmentIdValue).Length(LoanComparerConstants.IDNumberLength).Matches(@"^[a-zA-Z]{3}\d{6}$");
            }); // tez tu jest jakas zwalona logika

            When(x => x.GovernmentIdType == "Passport Number", () =>
            {
                RuleFor(x => x.GovernmentIdValue).Length(LoanComparerConstants.PassportNumberLength).Matches(@"^[a-zA-Z]{2}\d{7}$");
            }); // a tu to nawet nie wiem bo nie ma za duzo o tym

            RuleFor(x => x.Password).Equal(x => x.ConfirmPassword);
        }
    }
}
