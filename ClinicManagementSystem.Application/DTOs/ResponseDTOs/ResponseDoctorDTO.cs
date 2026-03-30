using ClinicManagementSystem.Application.DTOs.CreateDTOs.Abstract;

namespace ClinicManagementSystem.Application.DTOs.ResponseDTOs
{
    public class ResponseDoctorDTO : CreatePersonDTO
    {
        public int DoctorSpecializationId { get; set; }
        public string DoctorSpecializationName { get; set; } = string.Empty;
    }
}
