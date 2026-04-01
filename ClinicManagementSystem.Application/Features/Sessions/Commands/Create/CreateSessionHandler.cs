using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Commands.Create
{
    // Commands/Create
    public record CreateSessionCommand(CreateSessionDTO Dto) : IRequest<ResponseSessionDTO>;

    public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, ResponseSessionDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSessionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseSessionDTO> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            var appointmentExists = await _unitOfWork.Appointments.AnyAsync(a => a.Id == request.Dto.AppointmentId);
            if (!appointmentExists)
                throw new NotFoundException(nameof(Appointment), request.Dto.AppointmentId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == request.Dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), request.Dto.PatientId);

            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == request.Dto.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), request.Dto.DoctorId);

            var entity = _mapper.Map<Session>(request.Dto);

            await _unitOfWork.Sessions.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(entity.DoctorId);

            entity.Patient = patient;
            entity.Doctor = doctor;

            return _mapper.Map<ResponseSessionDTO>(entity);
        }
    }

    public class CreateSessionValidator : AbstractValidator<CreateSessionCommand>
    {
        public CreateSessionValidator()
        {
            RuleFor(x => x.Dto.AppointmentId)
                .GreaterThan(0).WithMessage("AppointmentId must be a valid id.");

            RuleFor(x => x.Dto.PatientId)
                .GreaterThan(0).WithMessage("PatientId must be a valid id.");

            RuleFor(x => x.Dto.DoctorId)
                .GreaterThan(0).WithMessage("DoctorId must be a valid id.");
        }
    }
}
