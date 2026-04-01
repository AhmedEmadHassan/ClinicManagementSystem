using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Commands.Create
{
    // Commands/Create
    public record CreateBillingCommand(CreateBillingDTO Dto) : IRequest<ResponseBillingDTO>;

    public class CreateBillingHandler : IRequestHandler<CreateBillingCommand, ResponseBillingDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateBillingHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseBillingDTO> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
        {
            var sessionExists = await _unitOfWork.Sessions.AnyAsync(s => s.Id == request.Dto.SessionId);
            if (!sessionExists)
                throw new NotFoundException(nameof(Session), request.Dto.SessionId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == request.Dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), request.Dto.PatientId);

            var entity = _mapper.Map<Billing>(request.Dto);

            await _unitOfWork.Billings.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            entity.Patient = patient;

            return _mapper.Map<ResponseBillingDTO>(entity);
        }
    }

    public class CreateBillingValidator : AbstractValidator<CreateBillingCommand>
    {
        public CreateBillingValidator()
        {
            RuleFor(x => x.Dto.SessionId)
                .GreaterThan(0).WithMessage("SessionId must be a valid id.");

            RuleFor(x => x.Dto.PatientId)
                .GreaterThan(0).WithMessage("PatientId must be a valid id.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }
}
