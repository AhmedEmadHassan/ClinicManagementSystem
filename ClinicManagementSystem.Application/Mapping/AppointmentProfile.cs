using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Mapping
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, ResponseAppointmentDTO>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.Name))
                .ForMember(dest => dest.AppointmentStateName, opt => opt.MapFrom(src => src.AppointmentState.Name));

            CreateMap<CreateAppointmentDTO, Appointment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateAppointmentDTO, Appointment>();
        }
    }
}
