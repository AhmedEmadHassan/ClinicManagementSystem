using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Mapping
{
    public class BillingProfile : Profile
    {
        public BillingProfile()
        {
            CreateMap<Billing, ResponseBillingDTO>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name));

            CreateMap<CreateBillingDTO, Billing>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            CreateMap<UpdateBillingDTO, Billing>();
        }
    }
}
