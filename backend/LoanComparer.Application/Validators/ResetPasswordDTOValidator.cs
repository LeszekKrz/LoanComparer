using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.UserDTO;

namespace LoanComparer.Application.Validators
{
    public class ResetPasswordDTOValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordDTOValidator()
        {
            RuleFor(x => x.Password).NotNull().NotEmpty();
            RuleFor(x => x.ConfirmPassword).NotNull().NotEmpty();

            RuleFor(x => x.Password)
                .Equal(x => x.ConfirmPassword)
                .WithMessage("Password and Confirm Password are not the same");

            RuleFor(x => x.Email).NotNull().Length(1, LoanComparerConstants.MaxEmailLength).EmailAddress();

            RuleFor(x => x.Token).NotNull().NotEmpty();
        }
    }
}
