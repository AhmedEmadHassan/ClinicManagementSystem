using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Commands.Create
{
    public record CreateBillingCommand(CreateBillingDTO Dto) : IRequest<ResponseBillingDTO>;

    public class CreateBillingHandler
    : IRequestHandler<CreateBillingCommand, ResponseBillingDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public CreateBillingHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseBillingDTO> Handle(
            CreateBillingCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(request.Dto.SessionId);
            if (session is null)
                throw new NotFoundException(nameof(Session), request.Dto.SessionId);

            var entity = _mapper.Map<Billing>(request.Dto);
            entity.PatientId = session.PatientId;

            await _unitOfWork.Billings.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            entity.Patient = patient;

            _cache.RemoveByPrefix(CacheKeys.Billing);

            return _mapper.Map<ResponseBillingDTO>(entity);
        }
    }

    public class CreateBillingValidator : AbstractValidator<CreateBillingCommand>
    {
        public CreateBillingValidator()
        {
            RuleFor(x => x.Dto.SessionId)
                .GreaterThan(0).WithMessage("SessionId must be a valid id.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }
}
