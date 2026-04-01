using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Commands.Update
{
    // Commands/Update
    public record UpdateAppointmentCommand(int Id, CreateAppointmentDTO Dto) : IRequest<ResponseAppointmentDTO>;

    public class UpdateAppointmentHandler : IRequestHandler<UpdateAppointmentCommand, ResponseAppointmentDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAppointmentHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseAppointmentDTO> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id);

            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), request.Id);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == request.Dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), request.Dto.PatientId);

            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == request.Dto.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), request.Dto.DoctorId);

            var stateExists = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Id == request.Dto.AppointmentStateId);
            if (!stateExists)
                throw new NotFoundException(nameof(AppointmentState), request.Dto.AppointmentStateId);

            _mapper.Map(request.Dto, appointment);

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
    }

    public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentValidator()
        {
            RuleFor(x => x.Dto.PatientId)
                .GreaterThan(0).WithMessage("PatientId must be a valid id.");

            RuleFor(x => x.Dto.DoctorId)
                .GreaterThan(0).WithMessage("DoctorId must be a valid id.");

            RuleFor(x => x.Dto.AppointmentStateId)
                .GreaterThan(0).WithMessage("AppointmentStateId must be a valid id.");

            RuleFor(x => x.Dto.AppointmentDate)
                .NotEmpty().WithMessage("AppointmentDate is required.");
        }
    }
}
