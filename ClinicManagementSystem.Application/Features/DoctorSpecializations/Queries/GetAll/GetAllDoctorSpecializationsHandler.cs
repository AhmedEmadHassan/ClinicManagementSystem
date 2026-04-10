using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetAll
{
    public record GetAllDoctorSpecializationsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseDoctorSpecializationDTO>>;

    public class GetAllDoctorSpecializationsHandler
    : IRequestHandler<GetAllDoctorSpecializationsQuery, PaginatedResponse<ResponseDoctorSpecializationDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetAllDoctorSpecializationsHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseDoctorSpecializationDTO>> Handle(
            GetAllDoctorSpecializationsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetAll(CacheKeys.DoctorSpecialization) + $":{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
            var cached = _cache.Get<PaginatedResponse<ResponseDoctorSpecializationDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var paged = await _unitOfWork.DoctorSpecializations.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                s => new ResponseDoctorSpecializationDTO { Id = s.Id, Name = s.Name });

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
