namespace LoanComparer.Application.DTO.UserDTO
{
    public record UserForRegistrationDTO(string FirstName, string LastName, string Email, JobTypeDTO JobType, int IncomeLevel, string GovernmentIdType,
        string GovernmentIdValue, string Password, string ConfirmPassword);
}
