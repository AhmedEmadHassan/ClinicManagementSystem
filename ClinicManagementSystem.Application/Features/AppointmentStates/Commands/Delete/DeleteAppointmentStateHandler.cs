using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Delete
{
    // Commands/Delete
    public record DeleteAppointmentStateCommand(int Id) : IRequest<bool>;

    public class DeleteAppointmentStateHandler
    : IRequestHandler<DeleteAppointmentStateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public DeleteAppointmentStateHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<bool> Handle(
            DeleteAppointmentStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(request.Id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), request.Id);

            await _unitOfWork.AppointmentStates.DeleteAsync(state);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.AppointmentState);

            return true;
        }
    }
}
