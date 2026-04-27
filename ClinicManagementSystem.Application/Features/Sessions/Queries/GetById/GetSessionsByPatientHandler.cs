using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Queries.GetById
{
    // GetSessionsByPatientQuery
    public record GetSessionsByPatientQuery(int PatientId, PaginationRequest Pagination)
        : IRequest<PaginatedResponse<ResponseSessionDTO>>;

    public class GetSessionsByPatientHandler
        : IRequestHandler<GetSessionsByPatientQuery, PaginatedResponse<ResponseSessionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetSessionsByPatientHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseSessionDTO>> Handle(
            GetSessionsByPatientQuery request, CancellationToken cancellationToken)
        {
            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == request.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), request.PatientId);

            var cacheKey = $"Session:ByPatient:{request.PatientId}:{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
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
                s => s.PatientId == request.PatientId);

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
