using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Commands.Update
{
    // StartSessionFromAppointmentCommand
    public record StartSessionFromAppointmentCommand(int AppointmentId) : IRequest<ResponseSessionDTO>;

    public class StartSessionFromAppointmentHandler
        : IRequestHandler<StartSessionFromAppointmentCommand, ResponseSessionDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public StartSessionFromAppointmentHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseSessionDTO> Handle(
            StartSessionFromAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId);
            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), request.AppointmentId);

            var scheduledState = await _unitOfWork.AppointmentStates
                .FirstOrDefaultAsync(s => s.Name == "Scheduled");

            if (scheduledState is null)
                throw new NotFoundException("AppointmentState 'Scheduled'", 0);

            if (appointment.AppointmentStateId != scheduledState.Id)
                throw new BadRequestException("Only scheduled appointments can be started.");

            var existingSession = await _unitOfWork.Sessions.AnyAsync(s => s.AppointmentId == request.AppointmentId);
            if (existingSession)
                throw new DuplicateException("A session already exists for this appointment.");

            var inProgressState = await _unitOfWork.AppointmentStates
                .FirstOrDefaultAsync(s => s.Name == "In Progress");

            if (inProgressState is not null)
            {
                appointment.AppointmentStateId = inProgressState.Id;
                await _unitOfWork.Appointments.UpdateAsync(appointment);
            }

            var entity = new Session
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId
            };

            await _unitOfWork.Sessions.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.Session);
            _cache.RemoveByPrefix(CacheKeys.Appointment);

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(entity.DoctorId);

            entity.Patient = patient;
            entity.Doctor = doctor;

            return _mapper.Map<ResponseSessionDTO>(entity);
        }
    }
}
