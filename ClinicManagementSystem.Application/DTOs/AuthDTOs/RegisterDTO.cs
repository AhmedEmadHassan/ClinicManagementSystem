namespace ClinicManagementSystem.Application.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin", "Doctor", "Patient", "Receptionist"

        // Required only if Role is "Doctor"
        public int? DoctorSpecializationId { get; set; }

        // Shared for Doctor and Patient
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Summary { get; set; }
    }
}
