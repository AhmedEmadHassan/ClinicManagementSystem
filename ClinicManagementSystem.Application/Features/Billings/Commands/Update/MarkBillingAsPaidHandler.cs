using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Commands.Update
{
    // MarkBillingAsPaidCommand
    public record MarkBillingAsPaidCommand(int Id) : IRequest<ResponseBillingDTO>;

    public class MarkBillingAsPaidHandler : IRequestHandler<MarkBillingAsPaidCommand, ResponseBillingDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public MarkBillingAsPaidHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseBillingDTO> Handle(
            MarkBillingAsPaidCommand request, CancellationToken cancellationToken)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(request.Id);
            if (billing is null)
                throw new NotFoundException(nameof(Billing), request.Id);

            if (billing.IsPaid)
                throw new BadRequestException("Billing is already marked as paid.");

            billing.IsPaid = true;

            await _unitOfWork.Billings.UpdateAsync(billing);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.Billing);

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);
            billing.Patient = patient;

            return _mapper.Map<ResponseBillingDTO>(billing);
        }
    }
}
