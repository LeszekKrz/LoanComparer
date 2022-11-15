using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.JwtFeatures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace LoanComparer.Application.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserForRegistrationDTO> _userForRegistrationValidator;
        private readonly IValidator<UserForAuthenticationDTO> _userForAuthenticationValidator;
        private readonly IValidator<ForgotPasswordDTO> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordDTO> _resetPasswordValidator;
        private readonly LoanComparerContext _context;
        private readonly JwtHandler _jwtHandler;
        private readonly EmailService _emailService;

        public UserService(UserManager<User> userManager, IValidator<UserForRegistrationDTO> userForRegistrationValidator,
            IValidator<UserForAuthenticationDTO> userForAuthenticationValidator, IValidator<ForgotPasswordDTO> forgotPasswordValidator,
            IValidator<ResetPasswordDTO> resetPasswordValidator, LoanComparerContext context, JwtHandler jwtHandler,
            EmailService emailService)
        {
            _userManager = userManager;
            _userForRegistrationValidator = userForRegistrationValidator;
            _userForAuthenticationValidator = userForAuthenticationValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _context = context;
            _jwtHandler = jwtHandler;
            _emailService = emailService;
        }

        public async Task<IEnumerable<ErrorResponseDTO>?> RegisterUserAsync(UserForRegistrationDTO userForRegistration, CancellationToken cancellationToken)
        {
            await _userForRegistrationValidator.ValidateAndThrowAsync(userForRegistration, cancellationToken);

            var newUser = new User(
                userForRegistration.FirstName,
                userForRegistration.LastName,
                userForRegistration.Email,
                await _context.JobTypes.SingleAsync(jobType => jobType.Name == userForRegistration.JobType.Name, cancellationToken),
                userForRegistration.IncomeLevel,
                new GovernmentId(userForRegistration.GovernmentId));

            IdentityResult result = await _userManager.CreateAsync(newUser, userForRegistration.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, LoanComparerConstants.ClientRoleName);
                return null;
            }

            return result.Errors.Select(e => new ErrorResponseDTO(e.Description));
        }

        public async Task<AuthenticationResponseDTO> LoginUserAsync(UserForAuthenticationDTO userForAuthentication, CancellationToken cancellationToken)
        {
            await _userForAuthenticationValidator.ValidateAndThrowAsync(userForAuthentication, cancellationToken);

            User user = await _userManager.FindByEmailAsync(userForAuthentication.Email);
            if (user == null)
                return new AuthenticationResponseDTO("There is no registered user with email provided", null);

            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
                return new AuthenticationResponseDTO("Provided password is invalid", null);

            SigningCredentials signingCredentials = _jwtHandler.GetSigningCredentials();
            ICollection<Claim> claims = await _jwtHandler.GetClaimsAsync(user);
            JwtSecurityToken jwtSecurityToken = _jwtHandler.GenerateJwtSecurityToken(signingCredentials, claims);
            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return new AuthenticationResponseDTO(null, token);
        }

        public async Task<ErrorResponseDTO?> ForgotPasswordAsync(ForgotPasswordDTO forgotPassword, CancellationToken cancellationToken)
        {
            await _forgotPasswordValidator.ValidateAndThrowAsync(forgotPassword, cancellationToken);

            User user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null)
                return new ErrorResponseDTO("There is no registered user with email provided");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var queryParameters = new Dictionary<string, string?>()
            {
                { "token", HttpUtility.UrlEncode(token) },
                { "email", forgotPassword.Email }
            };

            var callback = QueryHelpers.AddQueryString(forgotPassword.ClientURI, queryParameters);

            var email = new Email(
                new string[] { user.Email },
                LoanComparerConstants.PasswordResetEmailSubject,
                string.Empty,
                string.Format(LoanComparerConstants.PasswordResetHtmlContent, user.FirstName, callback));

            await _emailService.SendEmailAsync(email, cancellationToken);

            return null;
        }

        public async Task<IEnumerable<ErrorResponseDTO>?> ResetPasswordAsync(ResetPasswordDTO resetPassword, CancellationToken cancellationToken)
        {
            await _resetPasswordValidator.ValidateAndThrowAsync(resetPassword, cancellationToken);

            User user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
                return new ErrorResponseDTO[1] { new ErrorResponseDTO("There is no registered user with email provided") };
            
            IdentityResult resetPasswordResult = await _userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(resetPassword.Token), resetPassword.Password);

            return resetPasswordResult.Succeeded 
                ? null
                : resetPasswordResult.Errors.Select(error => new ErrorResponseDTO(error.Description));
        }
    }
}
