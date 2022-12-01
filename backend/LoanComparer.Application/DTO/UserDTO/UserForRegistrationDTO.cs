namespace LoanComparer.Application.DTO.UserDTO
{
    public record UserForRegistrationDTO(
        string FirstName,
        string LastName,
        string Email,
        JobTypeDTO JobType,
        int IncomeLevel,
        GovernmentIdDTO GovernmentId,
        string Password,
        string ConfirmPassword,
        string ClientURI);
}
