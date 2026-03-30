using ClinicManagementSystem.Application.DTOs.CreateDTOs.Abstract;

namespace ClinicManagementSystem.Application.DTOs.CreateDTOs
{
    public class DoctorCreateDTO : PersonCreateDTO
    {
        public int DoctorSpecializationId { get; set; }
    }
}
