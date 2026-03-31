using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services.Implementation
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseSessionDTO>> GetAll()
        {
            return await _unitOfWork.Sessions.GetAllAsync(s => new ResponseSessionDTO
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
        }

        public async Task<ResponseSessionDTO> GetById(int id)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id);

            if (session is null)
                throw new NotFoundException(nameof(Session), id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(session.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(session.DoctorId);

            return new ResponseSessionDTO
            {
                Id = session.Id,
                AppointmentId = session.AppointmentId,
                PatientId = session.PatientId,
                DoctorId = session.DoctorId,
                ConsultationNotes = session.ConsultationNotes,
                Prescriptions = session.Prescriptions,
                PatientName = patient?.Name ?? string.Empty,
                DoctorName = doctor?.Name ?? string.Empty
            };
        }

        public async Task<ResponseSessionDTO> Create(CreateSessionDTO dto)
        {
            var appointmentExists = await _unitOfWork.Appointments.AnyAsync(a => a.Id == dto.AppointmentId);
            if (!appointmentExists)
                throw new NotFoundException(nameof(Appointment), dto.AppointmentId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), dto.PatientId);

            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), dto.DoctorId);

            var entity = new Session
            {
                AppointmentId = dto.AppointmentId,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                ConsultationNotes = dto.ConsultationNotes,
                Prescriptions = dto.Prescriptions
            };

            await _unitOfWork.Sessions.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(entity.DoctorId);

            return new ResponseSessionDTO
            {
                Id = entity.Id,
                AppointmentId = entity.AppointmentId,
                PatientId = entity.PatientId,
                DoctorId = entity.DoctorId,
                ConsultationNotes = entity.ConsultationNotes,
                Prescriptions = entity.Prescriptions,
                PatientName = patient?.Name ?? string.Empty,
                DoctorName = doctor?.Name ?? string.Empty
            };
        }

        public async Task<ResponseSessionDTO> Update(int id, CreateSessionDTO dto)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id);

            if (session is null)
                throw new NotFoundException(nameof(Session), id);

            var appointmentExists = await _unitOfWork.Appointments.AnyAsync(a => a.Id == dto.AppointmentId);
            if (!appointmentExists)
                throw new NotFoundException(nameof(Appointment), dto.AppointmentId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), dto.PatientId);

            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), dto.DoctorId);

            session.AppointmentId = dto.AppointmentId;
            session.PatientId = dto.PatientId;
            session.DoctorId = dto.DoctorId;
            session.ConsultationNotes = dto.ConsultationNotes;
            session.Prescriptions = dto.Prescriptions;

            await _unitOfWork.Sessions.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(session.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(session.DoctorId);

            return new ResponseSessionDTO
            {
                Id = session.Id,
                AppointmentId = session.AppointmentId,
                PatientId = session.PatientId,
                DoctorId = session.DoctorId,
                ConsultationNotes = session.ConsultationNotes,
                Prescriptions = session.Prescriptions,
                PatientName = patient?.Name ?? string.Empty,
                DoctorName = doctor?.Name ?? string.Empty
            };
        }

        public async Task<bool> Delete(int id)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id);

            if (session is null)
                throw new NotFoundException(nameof(Session), id);

            await _unitOfWork.Sessions.DeleteAsync(session);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
