using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Update
{
    // Commands/Update
    public record UpdateDoctorSpecializationCommand(int Id, CreateDoctorSpecializationDTO Dto) : IRequest<ResponseDoctorSpecializationDTO>;

    public class UpdateDoctorSpecializationHandler : IRequestHandler<UpdateDoctorSpecializationCommand, ResponseDoctorSpecializationDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateDoctorSpecializationHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDoctorSpecializationDTO> Handle(UpdateDoctorSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(request.Id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), request.Id);

            var duplicate = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Name == request.Dto.Name && s.Id != request.Id);

            if (duplicate)
                throw new DuplicateException($"DoctorSpecialization with name '{request.Dto.Name}' already exists.");

            _mapper.Map(request.Dto, specialization);

            await _unitOfWork.DoctorSpecializations.UpdateAsync(specialization);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponseDoctorSpecializationDTO>(specialization);
        }
    }

    public class UpdateDoctorSpecializationValidator : AbstractValidator<UpdateDoctorSpecializationCommand>
    {
        public UpdateDoctorSpecializationValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
