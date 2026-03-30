using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Services.Abstraction.GenericInterface;

namespace ClinicManagementSystem.Application.Services.Abstraction
{
    public interface IDoctorSpecializationService : IGenericService<ResponseDoctorSpecializationDTO, CreateDoctorSpecializationDTO>
    {

    }
}
