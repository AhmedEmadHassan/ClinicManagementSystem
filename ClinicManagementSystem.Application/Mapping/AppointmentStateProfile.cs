using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Mapping
{
    public class AppointmentStateProfile : Profile
    {
        public AppointmentStateProfile()
        {
            CreateMap<AppointmentState, ResponseAppointmentStateDTO>();
            CreateMap<CreateAppointmentStateDTO, AppointmentState>();
        }
    }
}
