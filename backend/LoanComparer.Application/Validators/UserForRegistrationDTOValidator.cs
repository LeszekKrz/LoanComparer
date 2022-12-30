using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Validators
{
    public class UserForRegistrationDTOValidator : AbstractValidator<UserForRegistrationDTO>
    {
        public UserForRegistrationDTOValidator(LoanComparerContext context, UserManager<User> userManager)
        {
            RuleFor(x => x.FirstName).NotNull().Length(1, LoanComparerConstants.MaxFirstNameLength);

            RuleFor(x => x.LastName).NotNull().Length(1, LoanComparerConstants.MaxLastNameLength);

            RuleFor(x => x.Email)
                .NotNull()
                .Length(1, LoanComparerConstants.MaxEmailLength)
                .EmailAddress()
                .MustAsync(async (email, cancellationToken) => await userManager.FindByEmailAsync(email) == null)
                .WithMessage(x => $"User with email {x.Email} already exists");

            RuleFor(x => x.JobType)
                .MustAsync(async (jobType, cancellationToken) => await context.JobTypes.AnyAsync(jt => jt.Name == jobType.Name, cancellationToken))
                .WithMessage(x => $"No job type with name {x.JobType.Name} found");

            RuleFor(x => x.IncomeLevel).NotNull().GreaterThan(0);

            RuleFor(x => x.GovernmentId.Type).NotNull().NotEmpty()
                .Must(governmentIdType => LoanComparerConstants.GovernmentIdTypes.Contains(governmentIdType))
                .WithMessage("Invalid government id type");

            RuleFor(x => x.GovernmentId.Value)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (governmentIdValue, cancellationToken) 
                    => !await context.Users.AnyAsync(user => user.GovernmentIdEntity.Value == governmentIdValue))
                .WithMessage(x => $"User with specified government id already exists");

            When(x => x.GovernmentId.Type == "PESEL", () =>
            {
                RuleFor(x => x.GovernmentId.Value).Length(LoanComparerConstants.PeselLength).Matches(@"^\d{11}$"); // 11 jako argument jak?
            }); // jest jeszcze bardziej skomplikowana logika na pesel jakies tam dodawanie tych liczb ale to moze sie pozniej doda

            When(x => x.GovernmentId.Type == "ID Number", () =>
            {
                RuleFor(x => x.GovernmentId.Value).Length(LoanComparerConstants.IDNumberLength).Matches(@"^[a-zA-Z]{3}\d{6}$");
            }); // tez tu jest jakas zwalona logika

            When(x => x.GovernmentId.Type == "Passport Number", () =>
            {
                RuleFor(x => x.GovernmentId.Value).Length(LoanComparerConstants.PassportNumberLength).Matches(@"^[a-zA-Z]{2}\d{7}$");
            }); // a tu to nawet nie wiem bo nie ma za duzo o tym

            RuleFor(x => x.Password).NotNull().NotEmpty();
            RuleFor(x => x.ConfirmPassword).NotNull().NotEmpty();

            RuleFor(x => x.Password)
                .Equal(x => x.ConfirmPassword)
                .WithMessage("Password and Confirm Password are not the same");

            RuleFor(x => x.ClientURI)
                .NotNull()
                .NotEmpty()
                .Must(clientUri => Uri.TryCreate(clientUri, UriKind.Absolute, out _))
                .WithMessage("Internal Server error. Provided uri is invalid");
        }
    }
}
