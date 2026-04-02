using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
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

            CreateMap<CreateSessionDTO, Session>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore());

            CreateMap<UpdateSessionDTO, Session>();
        }
    }
}
