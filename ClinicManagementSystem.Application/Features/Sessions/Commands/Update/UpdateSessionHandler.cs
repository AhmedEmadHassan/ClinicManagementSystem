using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Commands.Update
{
    public record UpdateSessionCommand(int Id, UpdateSessionDTO Dto) : IRequest<ResponseSessionDTO>;

    public class UpdateSessionHandler
    : IRequestHandler<UpdateSessionCommand, ResponseSessionDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public UpdateSessionHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseSessionDTO> Handle(
            UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(request.Id);

            if (session is null)
                throw new NotFoundException(nameof(Session), request.Id);

            _mapper.Map(request.Dto, session);

            await _unitOfWork.Sessions.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(session.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(session.DoctorId);

            session.Patient = patient;
            session.Doctor = doctor;

            _cache.RemoveByPrefix(CacheKeys.Session);

            return _mapper.Map<ResponseSessionDTO>(session);
        }
    }

    public class UpdateSessionValidator : AbstractValidator<UpdateSessionCommand>
    {
        public UpdateSessionValidator()
        {
            RuleFor(x => x.Dto.ConsultationNotes)
                .MaximumLength(1000).WithMessage("ConsultationNotes must not exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.ConsultationNotes));

            RuleFor(x => x.Dto.Prescriptions)
                .MaximumLength(1000).WithMessage("Prescriptions must not exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Prescriptions));
        }
    }
}
