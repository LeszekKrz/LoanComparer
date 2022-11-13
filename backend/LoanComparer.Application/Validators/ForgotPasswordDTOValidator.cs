﻿using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.UserDTO;

namespace LoanComparer.Application.Validators
{
    public class ForgotPasswordDTOValidator : AbstractValidator<ForgotPasswordDTO>
    {
        public ForgotPasswordDTOValidator()
        {
            RuleFor(x => x.Email).NotNull().Length(1, LoanComparerConstants.MaxEmailLength);

            RuleFor(x => x.ClientURI).NotNull().NotEmpty();
        }
    }
}
