using FluentValidation;
using LoanComparer.Application.DTO;
using LoanComparer.Application.Exceptions;

namespace LoanComparer.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            catch (Exception)
            {
                HandleExceptionAsync(httpContext);
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

        private void HandleExceptionAsync(HttpContext httpContext) // logger is it okay?
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogInformation(
                "Request {method} {url} => {statusCode}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value,
                httpContext.Response.StatusCode);
        }
    }
}
