using ClinicManagementSystem.API.ResponseHandler;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers.Base
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, int statusCode = 200, string message = "Request completed successfully.")
        {
            return StatusCode(statusCode, ApiResponse<T>.Success(data, statusCode, message));
        }

        protected IActionResult Created<T>(T data, string message = "Resource created successfully.")
        {
            return StatusCode(StatusCodes.Status201Created, ApiResponse<T>.Success(data, StatusCodes.Status201Created, message));
        }

        protected IActionResult NoContent(string message = "Resource deleted successfully.")
        {
            return StatusCode(StatusCodes.Status200OK, ApiResponse<object>.Success(null, StatusCodes.Status200OK, message));
        }
    }
}
