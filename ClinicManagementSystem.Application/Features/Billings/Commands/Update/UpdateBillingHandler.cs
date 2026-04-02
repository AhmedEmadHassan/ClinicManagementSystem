using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Commands.Update
{
    public record UpdateBillingCommand(int Id, UpdateBillingDTO Dto) : IRequest<ResponseBillingDTO>;

    public class UpdateBillingHandler : IRequestHandler<UpdateBillingCommand, ResponseBillingDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateBillingHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseBillingDTO> Handle(UpdateBillingCommand request, CancellationToken cancellationToken)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(request.Id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), request.Id);

            _mapper.Map(request.Dto, billing);

            await _unitOfWork.Billings.UpdateAsync(billing);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);
            billing.Patient = patient;

            return _mapper.Map<ResponseBillingDTO>(billing);
        }
    }

    public class UpdateBillingValidator : AbstractValidator<UpdateBillingCommand>
    {
        public UpdateBillingValidator()
        {
            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }
}
