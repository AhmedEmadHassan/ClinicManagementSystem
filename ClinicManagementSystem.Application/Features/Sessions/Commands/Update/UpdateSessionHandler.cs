using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Commands.Update
{
    // Commands/Update
    public record UpdateSessionCommand(int Id, CreateSessionDTO Dto) : IRequest<ResponseSessionDTO>;

    public class UpdateSessionHandler : IRequestHandler<UpdateSessionCommand, ResponseSessionDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSessionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseSessionDTO> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(request.Id);

            if (session is null)
                throw new NotFoundException(nameof(Session), request.Id);

            var appointmentExists = await _unitOfWork.Appointments.AnyAsync(a => a.Id == request.Dto.AppointmentId);
            if (!appointmentExists)
                throw new NotFoundException(nameof(Appointment), request.Dto.AppointmentId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == request.Dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), request.Dto.PatientId);

            var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == request.Dto.DoctorId);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), request.Dto.DoctorId);

            _mapper.Map(request.Dto, session);

            await _unitOfWork.Sessions.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(session.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(session.DoctorId);

            session.Patient = patient;
            session.Doctor = doctor;

            return _mapper.Map<ResponseSessionDTO>(session);
        }
    }

    public class UpdateSessionValidator : AbstractValidator<UpdateSessionCommand>
    {
        public UpdateSessionValidator()
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
