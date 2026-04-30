using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Patients.Queries.GetById
{
    // GetPatientMedicalHistoryQuery
    public record GetPatientMedicalHistoryQuery(int PatientId) : IRequest<PatientMedicalHistoryDTO>;

    public class PatientMedicalHistoryDTO
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public List<ResponseAppointmentDTO> Appointments { get; set; } = new();
        public List<ResponseSessionDTO> Sessions { get; set; } = new();
        public List<ResponseBillingDTO> Billings { get; set; } = new();
    }

    public class GetPatientMedicalHistoryHandler
        : IRequestHandler<GetPatientMedicalHistoryQuery, PatientMedicalHistoryDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetPatientMedicalHistoryHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PatientMedicalHistoryDTO> Handle(
            GetPatientMedicalHistoryQuery request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId);
            if (patient is null)
                throw new NotFoundException(nameof(Patient), request.PatientId);

            var cacheKey = $"Patient:MedicalHistory:{request.PatientId}";
            var cached = _cache.Get<PatientMedicalHistoryDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var appointments = await _unitOfWork.Appointments.GetAllAsync(
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
                a => a.PatientId == request.PatientId);

            var sessions = await _unitOfWork.Sessions.GetAllAsync(
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

            var billings = await _unitOfWork.Billings.GetAllAsync(
                b => new ResponseBillingDTO
                {
                    Id = b.Id,
                    SessionId = b.SessionId,
                    PatientId = b.PatientId,
                    Description = b.Description,
                    Amount = b.Amount,
                    IsPaid = b.IsPaid,
                    PatientName = b.Patient.Name
                },
                b => b.PatientId == request.PatientId);

            var result = new PatientMedicalHistoryDTO
            {
                PatientId = patient.Id,
                PatientName = patient.Name,
                Appointments = appointments,
                Sessions = sessions,
                Billings = billings
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
    }
}
