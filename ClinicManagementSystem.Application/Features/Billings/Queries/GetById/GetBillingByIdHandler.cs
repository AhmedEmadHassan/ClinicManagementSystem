using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Queries.GetById
{
    // Queries/GetById
    public record GetBillingByIdQuery(int Id) : IRequest<ResponseBillingDTO>;

    public class GetBillingByIdHandler
    : IRequestHandler<GetBillingByIdQuery, ResponseBillingDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetBillingByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseBillingDTO> Handle(
            GetBillingByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetById(CacheKeys.Billing, request.Id);
            var cached = _cache.Get<ResponseBillingDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var billing = await _unitOfWork.Billings.GetByIdAsync(request.Id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), request.Id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);
            billing.Patient = patient;

            var dto = _mapper.Map<ResponseBillingDTO>(billing);
            _cache.Set(cacheKey, dto);
            return dto;
        }
    }
}
