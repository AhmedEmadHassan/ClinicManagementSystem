using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Patients.Queries.GetAll
{
    // Queries/GetAll
    public record GetAllPatientsQuery : IRequest<List<ResponsePatientDTO>>;

    public class GetAllPatientsHandler : IRequestHandler<GetAllPatientsQuery, List<ResponsePatientDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPatientsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponsePatientDTO>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            return _mapper.Map<List<ResponsePatientDTO>>(patients);
        }
    }
}
