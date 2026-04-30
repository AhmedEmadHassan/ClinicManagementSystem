using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Commands.Update
{
    // RescheduleAppointmentCommand
    public record RescheduleAppointmentCommand(int Id, RescheduleAppointmentDTO Dto)
        : IRequest<ResponseAppointmentDTO>;

    public class RescheduleAppointmentDTO
    {
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }
    }

    public class RescheduleAppointmentHandler : IRequestHandler<RescheduleAppointmentCommand, ResponseAppointmentDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public RescheduleAppointmentHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseAppointmentDTO> Handle(
            RescheduleAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id);
            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), request.Id);

            var cancelledState = await _unitOfWork.AppointmentStates
                .FirstOrDefaultAsync(s => s.Name == "Cancelled");

            if (cancelledState is not null && appointment.AppointmentStateId == cancelledState.Id)
                throw new BadRequestException("Cannot reschedule a cancelled appointment.");

            appointment.AppointmentDate = request.Dto.AppointmentDate;
            appointment.AppointmentTime = request.Dto.AppointmentTime;

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

    public class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointmentCommand>
    {
        public RescheduleAppointmentValidator()
        {
            RuleFor(x => x.Dto.AppointmentDate)
                .NotEmpty().WithMessage("AppointmentDate is required.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("AppointmentDate must be today or in the future.");
        }
    }
}
