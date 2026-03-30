using ClinicManagementSystem.Application.DTOs.ResponseDTOs.Abstract;

namespace ClinicManagementSystem.Application.DTOs.ResponseDTOs
{
    public class ResponseDoctorDTO : ResponsePersonDTO
    {
        public int DoctorSpecializationId { get; set; }
        public string DoctorSpecializationName { get; set; } = string.Empty;
    }
}
