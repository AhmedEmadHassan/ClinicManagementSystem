using AutoMapper;
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
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            appointment.Patient = patient;
            appointment.Doctor = doctor;
            appointment.AppointmentState = state;

            return _mapper.Map<ResponseAppointmentDTO>(appointment);
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

            var entity = _mapper.Map<Appointment>(dto);

            await _unitOfWork.Appointments.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(entity.DoctorId);
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(entity.AppointmentStateId);

            entity.Patient = patient;
            entity.Doctor = doctor;
            entity.AppointmentState = state;

            return _mapper.Map<ResponseAppointmentDTO>(entity);
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

            _mapper.Map(dto, appointment);

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(appointment.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId);
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(appointment.AppointmentStateId);

            appointment.Patient = patient;
            appointment.Doctor = doctor;
            appointment.AppointmentState = state;

            return _mapper.Map<ResponseAppointmentDTO>(appointment);
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
