using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetAll
{
    public record GetAllAppointmentStatesQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseAppointmentStateDTO>>;

    public class GetAllAppointmentStatesHandler
    : IRequestHandler<GetAllAppointmentStatesQuery, PaginatedResponse<ResponseAppointmentStateDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetAllAppointmentStatesHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseAppointmentStateDTO>> Handle(
            GetAllAppointmentStatesQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetAll(CacheKeys.AppointmentState) + $":{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
            var cached = _cache.Get<PaginatedResponse<ResponseAppointmentStateDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var paged = await _unitOfWork.AppointmentStates.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                s => new ResponseAppointmentStateDTO { Id = s.Id, Name = s.Name });

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
