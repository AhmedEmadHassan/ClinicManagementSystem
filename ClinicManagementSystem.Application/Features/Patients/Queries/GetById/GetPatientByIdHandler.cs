using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Patients.Queries.GetById
{
    // Queries/GetById
    public record GetPatientByIdQuery(int Id) : IRequest<ResponsePatientDTO>;

    public class GetPatientByIdHandler : IRequestHandler<GetPatientByIdQuery, ResponsePatientDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPatientByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponsePatientDTO> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), request.Id);

            return _mapper.Map<ResponsePatientDTO>(patient);
        }
    }
}
