using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Commands.Update
{
    // CancelAppointmentCommand
    public record CancelAppointmentCommand(int Id) : IRequest<ResponseAppointmentDTO>;

    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand, ResponseAppointmentDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public CancelAppointmentHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseAppointmentDTO> Handle(
            CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id);
            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), request.Id);

            var cancelledState = await _unitOfWork.AppointmentStates
                .FirstOrDefaultAsync(s => s.Name == "Cancelled");

            if (cancelledState is null)
                throw new NotFoundException("AppointmentState 'Cancelled'", 0);

            if (appointment.AppointmentStateId == cancelledState.Id)
                throw new BadRequestException("Appointment is already cancelled.");

            appointment.AppointmentStateId = cancelledState.Id;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.Appointment);

            var patient = await _unitOfWork.Patients.GetByIdAsync(appointment.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId);
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(appointment.AppointmentStateId);

            appointment.Patient = patient;
            appointment.Doctor = doctor;
            appointment.AppointmentState = state;

            return _mapper.Map<ResponseAppointmentDTO>(appointment);
        }
    }
}
