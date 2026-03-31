using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Mapping.Helpers;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Mapping
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, ResponseDoctorDTO>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => GenderHelper.Map(src.Gender)))
                .ForMember(dest => dest.DoctorSpecializationName, opt => opt.MapFrom(src => src.DoctorSpecialization.Name));

            CreateMap<CreateDoctorDTO, Doctor>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => GenderHelper.Parse(src.Gender)));
        }
    }
}
