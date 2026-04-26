namespace ClinicManagementSystem.Application.DTOs.ResponseDTOs
{
    public class ResponseUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
    }
}
