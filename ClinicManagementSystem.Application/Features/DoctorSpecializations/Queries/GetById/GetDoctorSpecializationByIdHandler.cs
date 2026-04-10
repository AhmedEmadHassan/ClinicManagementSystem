using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetById
{
    // Queries/GetById
    public record GetDoctorSpecializationByIdQuery(int Id) : IRequest<ResponseDoctorSpecializationDTO>;

    public class GetDoctorSpecializationByIdHandler
    : IRequestHandler<GetDoctorSpecializationByIdQuery, ResponseDoctorSpecializationDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetDoctorSpecializationByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseDoctorSpecializationDTO> Handle(
            GetDoctorSpecializationByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetById(CacheKeys.DoctorSpecialization, request.Id);
            var cached = _cache.Get<ResponseDoctorSpecializationDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(request.Id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), request.Id);

            var dto = _mapper.Map<ResponseDoctorSpecializationDTO>(specialization);
            _cache.Set(cacheKey, dto);
            return dto;
        }
    }
}
