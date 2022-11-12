using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Identity;

namespace LoanComparer.Application.Validators
{
    public class UserForAuthenticationDTOValidator : AbstractValidator<UserForAuthenticationDTO>
    {
        public UserForAuthenticationDTOValidator()
        {
            RuleFor(x => x.Email).NotNull().Length(1, LoanComparerConstants.MaxEmailLength);

            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}
