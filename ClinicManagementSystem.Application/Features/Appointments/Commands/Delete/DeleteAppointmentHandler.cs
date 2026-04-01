using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Commands.Delete
{
    // Commands/Delete
    public record DeleteAppointmentCommand(int Id) : IRequest<bool>;

    public class DeleteAppointmentHandler : IRequestHandler<DeleteAppointmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAppointmentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id);

            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), request.Id);

            await _unitOfWork.Appointments.DeleteAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
