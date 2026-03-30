using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Services.GenericInterface;

namespace ClinicManagementSystem.Application.Services
{
    public interface IAppointmentStateService : IGenericService<ResponseAppointmentStateDTO, CreateAppointmentStateDTO>
    {

    }
}
