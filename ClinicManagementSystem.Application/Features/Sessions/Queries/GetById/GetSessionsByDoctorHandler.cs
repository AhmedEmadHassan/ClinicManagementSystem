using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Queries.GetById
{
    // GetSessionsByDoctorQuery
    public record GetSessionsByDoctorQuery(int DoctorId, PaginationRequest Pagination)
        : IRequest<PaginatedResponse<ResponseSessionDTO>>;

    public class GetSessionsByDoctorHandler
        : IRequestHandler<GetSessionsByDoctorQuery, PaginatedResponse<ResponseSessionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetSessionsByDoctorHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseSessionDTO>> Handle(
            GetSessionsByDoctorQuery request, CancellationToken cancellationToken)
        {
            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == request.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), request.DoctorId);

            var cacheKey = $"Session:ByDoctor:{request.DoctorId}:{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
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
                },
                s => s.DoctorId == request.DoctorId);

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
