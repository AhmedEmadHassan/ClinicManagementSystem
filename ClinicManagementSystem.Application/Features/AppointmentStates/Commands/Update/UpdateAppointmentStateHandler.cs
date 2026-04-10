using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Update
{
    // Commands/Update
    public record UpdateAppointmentStateCommand(int Id, CreateAppointmentStateDTO Dto) : IRequest<ResponseAppointmentStateDTO>;

    public class UpdateAppointmentStateHandler
    : IRequestHandler<UpdateAppointmentStateCommand, ResponseAppointmentStateDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public UpdateAppointmentStateHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseAppointmentStateDTO> Handle(
            UpdateAppointmentStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(request.Id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), request.Id);

            var duplicate = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Name == request.Dto.Name && s.Id != request.Id);

            if (duplicate)
                throw new DuplicateException($"AppointmentState with name '{request.Dto.Name}' already exists.");

            _mapper.Map(request.Dto, state);

            await _unitOfWork.AppointmentStates.UpdateAsync(state);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.AppointmentState);

            return _mapper.Map<ResponseAppointmentStateDTO>(state);
        }
    }

    public class UpdateAppointmentStateValidator : AbstractValidator<UpdateAppointmentStateCommand>
    {
        public UpdateAppointmentStateValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
