using FluentValidation;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanComparer.Application.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserForRegistrationDTO> _userForRegistrationValidator;
        private readonly LoanComparerContext _context;

        public UserService(UserManager<User> userManager, IValidator<UserForRegistrationDTO> userForRegistrationValidator,
            LoanComparerContext context)
        {
            _userManager = userManager;
            _userForRegistrationValidator = userForRegistrationValidator;
            _context = context;
        }

        [HttpPost("registatrion")]
        public async Task<RegistrationResponseDTO> RegisterUser(UserForRegistrationDTO userForRegistration, CancellationToken cancellationToken)
        {
            await _userForRegistrationValidator.ValidateAndThrowAsync(userForRegistration, cancellationToken);

            var newUser = new User(
                userForRegistration.FirstName,
                userForRegistration.LastName,
                userForRegistration.Email,
                await _context.JobTypes.SingleAsync(jobType => jobType.Name == userForRegistration.JobType.Name, cancellationToken),
                userForRegistration.IncomeLevel,
                userForRegistration.GovernmentIdType,
                userForRegistration.GovernmentIdValue);

            IdentityResult result = await _userManager.CreateAsync(newUser, userForRegistration.Password);

            IEnumerable<string>? errors = null;

            if (!result.Succeeded)
                errors = result.Errors.Select(e => e.Description);

            return new RegistrationResponseDTO(errors);
        }
    }
}
