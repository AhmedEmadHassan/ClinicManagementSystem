using ClinicManagementSystem.API.ResponseHandler;
using ClinicManagementSystem.Application;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace ClinicManagementSystem.API.RateLimiting
{
    public static class RateLimitingDependencies
    {
        public static IServiceCollection AddRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = configuration
                .GetSection("RateLimitSettings")
                .Get<RateLimitSettings>()!;

            services.AddRateLimiter(options =>
            {
                // Global Fixed Window Policy
                options.AddFixedWindowLimiter("GlobalFixedWindow", opt =>
                {
                    opt.PermitLimit = settings.GlobalFixedWindow.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(settings.GlobalFixedWindow.WindowInSeconds);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                // Auth Sliding Window Policy
                options.AddSlidingWindowLimiter("AuthSlidingWindow", opt =>
                {
                    opt.PermitLimit = settings.AuthSlidingWindow.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(settings.AuthSlidingWindow.WindowInSeconds);
                    opt.SegmentsPerWindow = settings.AuthSlidingWindow.SegmentsPerWindow;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                // Global fallback — applies to all endpoints not explicitly decorated
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User?.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = settings.GlobalFixedWindow.PermitLimit,
                            Window = TimeSpan.FromSeconds(settings.GlobalFixedWindow.WindowInSeconds),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                // Custom rejection response
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(
                        ApiResponse<object>.Failure(
                            "Too many requests. Please slow down and try again later.",
                            StatusCodes.Status429TooManyRequests),
                        cancellationToken);
                };
            });

            return services;
        }
    }
}
