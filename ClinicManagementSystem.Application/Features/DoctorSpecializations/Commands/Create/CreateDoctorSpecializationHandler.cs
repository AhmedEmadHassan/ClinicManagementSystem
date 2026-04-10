using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;
namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Create
{
    // Commands/Create
    public record CreateDoctorSpecializationCommand(CreateDoctorSpecializationDTO Dto) : IRequest<ResponseDoctorSpecializationDTO>;

    public class CreateDoctorSpecializationHandler
    : IRequestHandler<CreateDoctorSpecializationCommand, ResponseDoctorSpecializationDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public CreateDoctorSpecializationHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseDoctorSpecializationDTO> Handle(
            CreateDoctorSpecializationCommand request, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Name == request.Dto.Name);

            if (exists)
                throw new DuplicateException($"DoctorSpecialization with name '{request.Dto.Name}' already exists.");

            var entity = _mapper.Map<DoctorSpecialization>(request.Dto);

            await _unitOfWork.DoctorSpecializations.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.DoctorSpecialization);

            return _mapper.Map<ResponseDoctorSpecializationDTO>(entity);
        }
    }

    public class CreateDoctorSpecializationValidator : AbstractValidator<CreateDoctorSpecializationCommand>
    {
        public CreateDoctorSpecializationValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
