using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Mapping
{
    public class DoctorSpecializationProfile : Profile
    {
        public DoctorSpecializationProfile()
        {
            CreateMap<DoctorSpecialization, ResponseDoctorSpecializationDTO>();
            CreateMap<CreateDoctorSpecializationDTO, DoctorSpecialization>();
        }
    }
}
