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
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Dto.AppointmentId);
            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), request.Dto.AppointmentId);

            var entity = _mapper.Map<Session>(request.Dto);
            entity.PatientId = appointment.PatientId;
            entity.DoctorId = appointment.DoctorId;

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
        }
    }
}
