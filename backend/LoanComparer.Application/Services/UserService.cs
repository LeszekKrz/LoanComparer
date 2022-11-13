using FluentValidation;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.JwtFeatures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoanComparer.Application.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserForRegistrationDTO> _userForRegistrationValidator;
        private readonly IValidator<UserForAuthenticationDTO> _userForAuthenticationValidator;
        private readonly LoanComparerContext _context;
        private readonly JwtHandler _jwtHandler;

        public UserService(UserManager<User> userManager, IValidator<UserForRegistrationDTO> userForRegistrationValidator,
            IValidator<UserForAuthenticationDTO> userForAuthenticationValidator, LoanComparerContext context, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _userForRegistrationValidator = userForRegistrationValidator;
            _userForAuthenticationValidator = userForAuthenticationValidator;
            _context = context;
            _jwtHandler = jwtHandler;
        }

        public async Task<IEnumerable<ErrorResponseDTO>?> RegisterUser(UserForRegistrationDTO userForRegistration, CancellationToken cancellationToken)
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

        public async Task<AuthenticationResponseDTO> LoginUser(UserForAuthenticationDTO userForAuthentication)
        {
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
    }
}
