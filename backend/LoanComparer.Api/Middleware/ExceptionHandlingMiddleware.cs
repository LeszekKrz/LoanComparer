using FluentValidation;
using Google.Apis.Auth;
using LoanComparer.Application.DTO;
using LoanComparer.Application.Exceptions;

namespace LoanComparer.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment webHostEnvironment)
        {
            _next = next;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException validationException)
            {
                await HandleValidationExceptionAsync(httpContext, validationException);
            }
            catch (BadRequestException badRequestException)
            {
                await HandleBadRequestException(httpContext, badRequestException);
            }
            catch (InvalidJwtException invalidJwtException)
            {
                await HandleInvalidJwtException(httpContext, invalidJwtException);
            }
            catch (Exception exception)
            {
                HandleException(httpContext, exception);
            }
        }

        private async Task HandleValidationExceptionAsync(HttpContext httpContext, ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var mappedErrors = validationException.Errors.Select(error => new ErrorResponseDTO(error.ErrorMessage));
            await httpContext.Response.WriteAsJsonAsync(mappedErrors);
        }

        private async Task HandleBadRequestException(HttpContext httpContext, BadRequestException badRequestException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var mappedErrors = badRequestException.Errors;
            await httpContext.Response.WriteAsJsonAsync(mappedErrors);
        }

        private async Task HandleInvalidJwtException(HttpContext httpContext, InvalidJwtException invalidJwtException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(new List<ErrorResponseDTO> { new(invalidJwtException.Message) });
        }

        private void HandleException(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            if (!_webHostEnvironment.IsProduction())
            {
                _logger.LogInformation(
                "Request {method} {url} => {statusCode}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value,
                httpContext.Response.StatusCode);
                _logger.LogInformation("Exception message: " + exception.Message);
                _logger.LogInformation("Exception source: " + exception.Source);
                _logger.LogInformation("Exception stack trace: " + exception.StackTrace);
            }
        }
    }
}
