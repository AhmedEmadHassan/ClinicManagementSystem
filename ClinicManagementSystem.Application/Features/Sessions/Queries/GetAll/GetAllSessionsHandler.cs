using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Queries.GetAll
{
    public record GetAllSessionsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseSessionDTO>>;

    public class GetAllSessionsHandler
    : IRequestHandler<GetAllSessionsQuery, PaginatedResponse<ResponseSessionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetAllSessionsHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseSessionDTO>> Handle(
            GetAllSessionsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetAll(CacheKeys.Session) + $":{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
            var cached = _cache.Get<PaginatedResponse<ResponseSessionDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var paged = await _unitOfWork.Sessions.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                s => new ResponseSessionDTO
                {
                    Id = s.Id,
                    AppointmentId = s.AppointmentId,
                    PatientId = s.PatientId,
                    DoctorId = s.DoctorId,
                    ConsultationNotes = s.ConsultationNotes,
                    Prescriptions = s.Prescriptions,
                    PatientName = s.Patient.Name,
                    DoctorName = s.Doctor.Name
                });

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
