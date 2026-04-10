using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Queries.GetAll
{
    public record GetAllAppointmentsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseAppointmentDTO>>;

    public class GetAllAppointmentsHandler
    : IRequestHandler<GetAllAppointmentsQuery, PaginatedResponse<ResponseAppointmentDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetAllAppointmentsHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseAppointmentDTO>> Handle(
            GetAllAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetAll(CacheKeys.Appointment) + $":{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
            var cached = _cache.Get<PaginatedResponse<ResponseAppointmentDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var paged = await _unitOfWork.Appointments.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                a => new ResponseAppointmentDTO
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    AppointmentStateId = a.AppointmentStateId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    PatientName = a.Patient.Name,
                    DoctorName = a.Doctor.Name,
                    AppointmentStateName = a.AppointmentState.Name
                });

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
