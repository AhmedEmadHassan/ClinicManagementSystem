using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Patients.Queries.GetById
{
    // Queries/GetById
    public record GetPatientByIdQuery(int Id) : IRequest<ResponsePatientDTO>;

    public class GetPatientByIdHandler
    : IRequestHandler<GetPatientByIdQuery, ResponsePatientDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetPatientByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponsePatientDTO> Handle(
            GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetById(CacheKeys.Patient, request.Id);
            var cached = _cache.Get<ResponsePatientDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), request.Id);

            var dto = _mapper.Map<ResponsePatientDTO>(patient);
            _cache.Set(cacheKey, dto);
            return dto;
        }
    }
}
