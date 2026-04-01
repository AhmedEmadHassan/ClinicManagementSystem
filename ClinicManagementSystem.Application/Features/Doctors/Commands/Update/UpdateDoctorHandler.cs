using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Doctors.Commands.Update
{
    // Commands/Update
    public record UpdateDoctorCommand(int Id, CreateDoctorDTO Dto) : IRequest<ResponseDoctorDTO>;

    public class UpdateDoctorHandler : IRequestHandler<UpdateDoctorCommand, ResponseDoctorDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateDoctorHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDoctorDTO> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.Id);

            if (doctor is null)
                throw new NotFoundException(nameof(Doctor), request.Id);

            var specializationExists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Id == request.Dto.DoctorSpecializationId);

            if (!specializationExists)
                throw new NotFoundException(nameof(DoctorSpecialization), request.Dto.DoctorSpecializationId);

            _mapper.Map(request.Dto, doctor);

            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(doctor.DoctorSpecializationId);
            doctor.DoctorSpecialization = specialization;

            return _mapper.Map<ResponseDoctorDTO>(doctor);
        }
    }

    public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorCommand>
    {
        public UpdateDoctorValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Dto.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");

            RuleFor(x => x.Dto.Gender)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => g.Trim().ToLower() == "male" || g.Trim().ToLower() == "female")
                .WithMessage("Gender must be 'Male' or 'Female'.");

            RuleFor(x => x.Dto.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));

            RuleFor(x => x.Dto.DoctorSpecializationId)
                .GreaterThan(0).WithMessage("DoctorSpecializationId must be a valid id.");
        }
    }
}
