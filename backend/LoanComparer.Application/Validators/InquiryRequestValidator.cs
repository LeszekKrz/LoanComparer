using FluentValidation;
using LoanComparer.Application.DTO.InquiryDTO;

namespace LoanComparer.Application.Validators;

public sealed class InquiryRequestValidator : AbstractValidator<InquiryRequest>
{
    public InquiryRequestValidator()
    {
        // TODO: Move request validation here
    }
}