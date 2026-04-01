using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Patients.Commands.Update
{
    // Commands/Update
    public record UpdatePatientCommand(int Id, CreatePatientDTO Dto) : IRequest<ResponsePatientDTO>;

    public class UpdatePatientHandler : IRequestHandler<UpdatePatientCommand, ResponsePatientDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePatientHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponsePatientDTO> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), request.Id);

            _mapper.Map(request.Dto, patient);

            await _unitOfWork.Patients.UpdateAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponsePatientDTO>(patient);
        }
    }

    public class UpdatePatientValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientValidator()
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
        }
    }
}
