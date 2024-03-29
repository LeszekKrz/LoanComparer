﻿using FluentValidation;
using Google.Apis.Auth;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO;
using LoanComparer.Application.DTO.UserDTO;
using LoanComparer.Application.Exceptions;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.JwtFeatures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Web;

namespace LoanComparer.Application.Services
{
    public sealed class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly LoanComparerContext _context;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailService _emailService;
        private readonly GoogleAuthenticationSettings _googleAuthenticationSettings;

        public UserService(
            UserManager<User> userManager,
            IServiceProvider serviceProvider,
            LoanComparerContext context,
            JwtHandler jwtHandler,
            IEmailService emailService,
            GoogleAuthenticationSettings googleAuthenticationSettings)
        {
            _userManager = userManager;
            _serviceProvider = serviceProvider;
            _context = context;
            _jwtHandler = jwtHandler;
            _emailService = emailService;
            _googleAuthenticationSettings = googleAuthenticationSettings;
        }

        public async Task RegisterUserAsync(UserForRegistrationDTO userForRegistration, CancellationToken cancellationToken)
        {
            await _serviceProvider.GetService<IValidator<UserForRegistrationDTO>>()
                .ValidateAndThrowAsync(userForRegistration, cancellationToken);

            var newUser = new User(
                userForRegistration.FirstName,
                userForRegistration.LastName,
                userForRegistration.Email,
                await _context.JobTypes.SingleAsync(jobType => jobType.Name == userForRegistration.JobType.Name,
                    cancellationToken),
                userForRegistration.IncomeLevel,
                new GovernmentIdEntity(userForRegistration.GovernmentId));

            var result = await _userManager.CreateAsync(newUser, userForRegistration.Password);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var queryParameters = new Dictionary<string, string?>
            {
                { "token", HttpUtility.UrlEncode(token) },
                { "email", newUser.Email }
            };
            var confirmationLink = QueryHelpers.AddQueryString(userForRegistration.ClientURI, queryParameters);

            var email = new ConfirmEmailAddressEmail(newUser.Email, newUser.FirstName, confirmationLink);
            await _emailService.SendEmailAsync(email, cancellationToken);

            await _userManager.AddToRoleAsync(newUser, LoanComparerConstants.ClientRoleName);
        }

        public async Task<AuthenticationResponseDTO> LoginUserAsync(UserForAuthenticationDTO userForAuthentication, CancellationToken cancellationToken)
        {
            await _serviceProvider.GetService<IValidator<UserForAuthenticationDTO>>().ValidateAndThrowAsync(userForAuthentication, cancellationToken);

            User user = await GetUserByEmailAsync(userForAuthentication.Email);

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new BadRequestException(new ErrorResponseDTO[1] { new ErrorResponseDTO("Email is not confirmed") });

            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
                throw new BadRequestException(new ErrorResponseDTO[1] { new ErrorResponseDTO("Provided password is invalid") });

            return new AuthenticationResponseDTO(await _jwtHandler.GenerateTokenAsync(user));
        }

        public async Task<AuthenticationResponseDTO> LoginUserWithGoogleAsync(UserForGoogleAuthenticationDTO userForGoogleAuthentication)
        {
            GoogleJsonWebSignature.Payload payload = await ValidateGoogleCredentialAsync(userForGoogleAuthentication);
            UserLoginInfo userLoginInfo = new("GOOGLE", payload.Subject, "GOOGLE");
            User user = await _userManager.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);

            if (user != null)
                return new AuthenticationResponseDTO(await _jwtHandler.GenerateTokenAsync(user));

            user = await _userManager.FindByEmailAsync(payload.Email);
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, userLoginInfo);
                return new AuthenticationResponseDTO(await _jwtHandler.GenerateTokenAsync(user));
            }

            user = await CreateNewGoogleUser(payload, userLoginInfo);
            return new AuthenticationResponseDTO(await _jwtHandler.GenerateTokenAsync(user));
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleCredentialAsync(UserForGoogleAuthenticationDTO userForGoogleAuthentication)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _googleAuthenticationSettings.ClientId, }
            };

            return await GoogleJsonWebSignature.ValidateAsync(userForGoogleAuthentication.credential, settings);
        }

        private async Task<User> CreateNewGoogleUser(GoogleJsonWebSignature.Payload payload, UserLoginInfo userLoginInfo)
        {
            User user = new()
            {
                Email = payload.Email,
                UserName = payload.Email
            };
            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, LoanComparerConstants.ClientRoleName);
            await _userManager.AddLoginAsync(user, userLoginInfo);

            return user;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO forgotPassword, CancellationToken cancellationToken)
        {
            await _serviceProvider.GetService<IValidator<ForgotPasswordDTO>>()
                .ValidateAndThrowAsync(forgotPassword, cancellationToken);

            var user = await GetUserByEmailAsync(forgotPassword.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var queryParameters = new Dictionary<string, string?>
            {
                { "token", HttpUtility.UrlEncode(token) },
                { "email", forgotPassword.Email }
            };
            var callback = QueryHelpers.AddQueryString(forgotPassword.ClientURI, queryParameters);

            var email = new ResetPasswordEmail(user.Email, user.FirstName, callback);
            await _emailService.SendEmailAsync(email, cancellationToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO resetPassword, CancellationToken cancellationToken)
        {
            await _serviceProvider.GetService<IValidator<ResetPasswordDTO>>().ValidateAndThrowAsync(resetPassword, cancellationToken);

            var user = await GetUserByEmailAsync(resetPassword.Email);
            
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(resetPassword.Token), resetPassword.Password);

            if (!resetPasswordResult.Succeeded)
                throw new BadRequestException(resetPasswordResult.Errors.Select(error => new ErrorResponseDTO(error.Description)));
        }

        public async Task ConfirmEmailAsync(string email, string token)
        {
            var user = await GetUserByEmailAsync(email);

            var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(token));

            if (!confirmEmailResult.Succeeded)
                throw new BadRequestException(confirmEmailResult.Errors.Select(error => new ErrorResponseDTO(error.Description)));
        }

        private async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new BadRequestException(new ErrorResponseDTO[] { new("There is no registered user with email provided") });
            return user;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<UserInfoDTO> GetUserInfoAsync(string username)
        {
            var user = await _context.Users
                .Include(user => user.JobType)
                .Include(user => user.GovernmentIdEntity)
                .SingleOrDefaultAsync(user => user.UserName == username);
            if (user == null)
                throw new Exception("There is no registered user with username provided");

            return new(
                user.FirstName,
                user.LastName,
                user.Email,
                user.JobType?.Name ?? null,
                user.IncomeLevel,
                user.GovernmentIdEntity?.Type ?? null,
                user.GovernmentIdEntity?.Value ?? null);
        }
    }
}
