using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Queries.GetById
{
    // GetAppointmentsByDoctorQuery
    public record GetAppointmentsByDoctorQuery(int DoctorId, PaginationRequest Pagination)
        : IRequest<PaginatedResponse<ResponseAppointmentDTO>>;

    public class GetAppointmentsByDoctorHandler
        : IRequestHandler<GetAppointmentsByDoctorQuery, PaginatedResponse<ResponseAppointmentDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetAppointmentsByDoctorHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseAppointmentDTO>> Handle(
            GetAppointmentsByDoctorQuery request, CancellationToken cancellationToken)
        {
            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == request.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), request.DoctorId);

            var cacheKey = $"Appointment:ByDoctor:{request.DoctorId}:{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
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
                a => a.DoctorId == request.DoctorId);

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
