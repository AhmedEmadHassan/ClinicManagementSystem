using ClinicManagementSystem.API.ResponseHandler;
using ClinicManagementSystem.Application.Exceptions;

namespace ClinicManagementSystem.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException ex:
                    statusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                    break;

                case BadRequestException ex:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
                    break;

                case DuplicateException ex:
                    statusCode = StatusCodes.Status409Conflict;
                    message = ex.Message;
                    _logger.LogWarning(ex, "Duplicate: {Message}", ex.Message);
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Something went wrong.";
                    _logger.LogError(exception, "Unhandled exception at {Path}", context.Request.Path);
                    break;
            }

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Failure(message, statusCode)
            );
        }
    }
}
