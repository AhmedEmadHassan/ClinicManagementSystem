using ClinicManagementSystem.Application.DTOs.CreateDTOs.Abstract;

namespace ClinicManagementSystem.Application.DTOs.CreateDTOs
{
    public class CreateDoctorDTO : CreatePersonDTO
    {
        public int DoctorSpecializationId { get; set; }
    }
}
