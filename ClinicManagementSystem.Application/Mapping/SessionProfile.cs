using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Mapping
{
    public class SessionProfile : Profile
    {
        public SessionProfile()
        {
            CreateMap<Session, ResponseSessionDTO>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.Name));

            CreateMap<CreateSessionDTO, Session>();
        }
    }
}
