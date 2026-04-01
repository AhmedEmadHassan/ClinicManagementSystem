using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Patients.Commands.Create
{
    // Commands/Create
    public record CreatePatientCommand(CreatePatientDTO Dto) : IRequest<ResponsePatientDTO>;

    public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, ResponsePatientDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePatientHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponsePatientDTO> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Patient>(request.Dto);

            await _unitOfWork.Patients.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponsePatientDTO>(entity);
        }
    }

    public class CreatePatientValidator : AbstractValidator<CreatePatientCommand>
    {
        public CreatePatientValidator()
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
                .Must(g => g.Trim().Equals("male", StringComparison.OrdinalIgnoreCase) ||
                           g.Trim().Equals("female", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Gender must be 'Male' or 'Female'.");

            RuleFor(x => x.Dto.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));
        }
    }
}
