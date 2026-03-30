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
        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int StatusCode;
            string Message;

            switch (exception)
            {
                case NotFoundException NotFoundEx:
                    StatusCode = StatusCodes.Status404NotFound;
                    Message = NotFoundEx.Message;
                    break;
                case BadRequestException BadRequestEx:
                    StatusCode = StatusCodes.Status400BadRequest;
                    Message = BadRequestEx.Message;
                    break;
                case DuplicateException DuplicateEx:
                    StatusCode = StatusCodes.Status409Conflict;
                    Message = DuplicateEx.Message;
                    break;

                default:
                    StatusCode = StatusCodes.Status500InternalServerError;
                    Message = "Something Went Wrong!";
                    break;
            }

            context.Response.StatusCode = StatusCode;
            await context.Response.WriteAsJsonAsync(
                new
                {
                    status = StatusCode,
                    message = Message
                }
                );
        }
    }
}
