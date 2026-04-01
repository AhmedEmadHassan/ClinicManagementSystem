using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Queries.GetById
{
    // Queries/GetById
    public record GetBillingByIdQuery(int Id) : IRequest<ResponseBillingDTO>;

    public class GetBillingByIdHandler : IRequestHandler<GetBillingByIdQuery, ResponseBillingDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBillingByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseBillingDTO> Handle(GetBillingByIdQuery request, CancellationToken cancellationToken)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(request.Id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), request.Id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);
            billing.Patient = patient;

            return _mapper.Map<ResponseBillingDTO>(billing);
        }
    }
}
