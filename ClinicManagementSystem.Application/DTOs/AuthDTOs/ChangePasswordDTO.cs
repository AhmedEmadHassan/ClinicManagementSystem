namespace ClinicManagementSystem.Application.DTOs.AuthDTOs
{
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
