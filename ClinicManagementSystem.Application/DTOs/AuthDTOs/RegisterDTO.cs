namespace ClinicManagementSystem.Application.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Shared for Doctor and Patient
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Summary { get; set; }
    }
}
