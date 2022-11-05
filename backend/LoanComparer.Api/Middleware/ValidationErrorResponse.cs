namespace LoanComparer.Api.Middleware
{
    public record ValidationErrorResponse(string PropertyName, string ErrorCode, string ErrorMessage);
}
