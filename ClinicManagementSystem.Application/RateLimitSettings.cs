namespace ClinicManagementSystem.Application
{
    public class RateLimitSettings
    {
        public RateLimitPolicy GlobalFixedWindow { get; set; } = new();
        public RateLimitPolicy AuthSlidingWindow { get; set; } = new();
    }

    public class RateLimitPolicy
    {
        public int PermitLimit { get; set; }
        public int WindowInSeconds { get; set; }
        public int SegmentsPerWindow { get; set; }
    }
}
