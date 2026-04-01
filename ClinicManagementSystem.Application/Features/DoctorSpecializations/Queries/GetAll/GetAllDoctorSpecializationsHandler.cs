using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetAll
{
    // Queries/GetAll
    public record GetAllDoctorSpecializationsQuery : IRequest<List<ResponseDoctorSpecializationDTO>>;

    public class GetAllDoctorSpecializationsHandler : IRequestHandler<GetAllDoctorSpecializationsQuery, List<ResponseDoctorSpecializationDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllDoctorSpecializationsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponseDoctorSpecializationDTO>> Handle(GetAllDoctorSpecializationsQuery request, CancellationToken cancellationToken)
        {
            var specializations = await _unitOfWork.DoctorSpecializations.GetAllAsync();
            return _mapper.Map<List<ResponseDoctorSpecializationDTO>>(specializations);
        }
    }
}
