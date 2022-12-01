namespace LoanComparer.Application.DTO.UserDTO
{
    public record ResetPasswordDTO(string Password, string ConfirmPassword, string Email, string Token);
}
