using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Queries.GetAll
{
    // GetTodaysAppointmentsQuery
    public record GetTodaysAppointmentsQuery(PaginationRequest Pagination)
        : IRequest<PaginatedResponse<ResponseAppointmentDTO>>;

    public class GetTodaysAppointmentsHandler
        : IRequestHandler<GetTodaysAppointmentsQuery, PaginatedResponse<ResponseAppointmentDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetTodaysAppointmentsHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseAppointmentDTO>> Handle(
            GetTodaysAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var cacheKey = $"Appointment:Today:{today}:{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
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
                },
                a => a.AppointmentDate == today);

            _cache.Set(cacheKey, paged, TimeSpan.FromMinutes(5));
            return paged;
        }
    }
}
