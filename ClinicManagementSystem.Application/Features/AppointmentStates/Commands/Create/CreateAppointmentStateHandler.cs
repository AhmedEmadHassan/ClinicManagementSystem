using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Create
{
    // Commands/Create
    public record CreateAppointmentStateCommand(CreateAppointmentStateDTO Dto) : IRequest<ResponseAppointmentStateDTO>;

    public class CreateAppointmentStateHandler : IRequestHandler<CreateAppointmentStateCommand, ResponseAppointmentStateDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAppointmentStateHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseAppointmentStateDTO> Handle(CreateAppointmentStateCommand request, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Name == request.Dto.Name);

            if (exists)
                throw new DuplicateException($"AppointmentState with name '{request.Dto.Name}' already exists.");

            var entity = _mapper.Map<AppointmentState>(request.Dto);

            await _unitOfWork.AppointmentStates.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponseAppointmentStateDTO>(entity);
        }
    }

    public class CreateAppointmentStateValidator : AbstractValidator<CreateAppointmentStateCommand>
    {
        public CreateAppointmentStateValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
