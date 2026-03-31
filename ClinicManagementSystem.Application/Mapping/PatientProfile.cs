using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Mapping.Helpers;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Mapping
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, ResponsePatientDTO>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => GenderHelper.Map(src.Gender)));

            CreateMap<CreatePatientDTO, Patient>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => GenderHelper.Parse(src.Gender)));
        }
    }
}
