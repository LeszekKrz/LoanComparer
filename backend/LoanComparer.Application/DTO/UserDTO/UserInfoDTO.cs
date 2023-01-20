namespace LoanComparer.Application.DTO.UserDTO
{
    public sealed record UserInfoDTO(
        string? FirstName,
        string? LastName,
        string? Email,
        string? JobType,
        int? IncomeLevel,
        string? GovernmentIdType,
        string? GovernmentIdValue);
}
