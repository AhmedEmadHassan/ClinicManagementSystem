namespace ClinicManagementSystem.API.ResponseHandler
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, int statusCode = 200, string message = "Request completed successfully.")
        {
            return new ApiResponse<T>
            {
                Succeeded = true,
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
        }

        public static ApiResponse<T> Failure(string message, int statusCode)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                Message = message,
                StatusCode = statusCode,
                Data = default
            };
        }
    }
}
