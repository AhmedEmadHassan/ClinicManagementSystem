using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseAppointmentDTO>> GetAll()
        {
            return await _unitOfWork.Appointments.GetAllAsync(a => new ResponseAppointmentDTO
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
        }

        public async Task<ResponseAppointmentDTO> GetById(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(appointment.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId);
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(appointment.AppointmentStateId);

            return new ResponseAppointmentDTO
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentStateId = appointment.AppointmentStateId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt,
                PatientName = patient?.Name ?? string.Empty,
                DoctorName = doctor?.Name ?? string.Empty,
                AppointmentStateName = state?.Name ?? string.Empty
            };
        }

        public async Task<ResponseAppointmentDTO> Create(CreateAppointmentDTO dto)
        {
            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), dto.PatientId);

            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), dto.DoctorId);

            var stateExists = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Id == dto.AppointmentStateId);
            if (!stateExists)
                throw new NotFoundException(nameof(AppointmentState), dto.AppointmentStateId);

            var entity = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentStateId = dto.AppointmentStateId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(entity.DoctorId);
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(entity.AppointmentStateId);

            return new ResponseAppointmentDTO
            {
                Id = entity.Id,
                PatientId = entity.PatientId,
                DoctorId = entity.DoctorId,
                AppointmentStateId = entity.AppointmentStateId,
                AppointmentDate = entity.AppointmentDate,
                AppointmentTime = entity.AppointmentTime,
                Notes = entity.Notes,
                CreatedAt = entity.CreatedAt,
                PatientName = patient?.Name ?? string.Empty,
                DoctorName = doctor?.Name ?? string.Empty,
                AppointmentStateName = state?.Name ?? string.Empty
            };
        }

        public async Task<ResponseAppointmentDTO> Update(int id, CreateAppointmentDTO dto)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), id);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), dto.PatientId);

            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), dto.DoctorId);

            var stateExists = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Id == dto.AppointmentStateId);
            if (!stateExists)
                throw new NotFoundException(nameof(AppointmentState), dto.AppointmentStateId);

            appointment.PatientId = dto.PatientId;
            appointment.DoctorId = dto.DoctorId;
            appointment.AppointmentStateId = dto.AppointmentStateId;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.AppointmentTime = dto.AppointmentTime;
            appointment.Notes = dto.Notes;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(appointment.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId);
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(appointment.AppointmentStateId);

            return new ResponseAppointmentDTO
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentStateId = appointment.AppointmentStateId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt,
                PatientName = patient?.Name ?? string.Empty,
                DoctorName = doctor?.Name ?? string.Empty,
                AppointmentStateName = state?.Name ?? string.Empty
            };
        }

        public async Task<bool> Delete(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), id);

            await _unitOfWork.Appointments.DeleteAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
