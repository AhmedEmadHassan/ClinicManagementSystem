using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Commands.Delete
{
    // Commands/Delete
    public record DeleteSessionCommand(int Id) : IRequest<bool>;

    public class DeleteSessionHandler : IRequestHandler<DeleteSessionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSessionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(request.Id);

            if (session is null)
                throw new NotFoundException(nameof(Session), request.Id);

            await _unitOfWork.Sessions.DeleteAsync(session);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
