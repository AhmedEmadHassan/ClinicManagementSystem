using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Commands.Delete
{
    // Commands/Delete
    public record DeleteBillingCommand(int Id) : IRequest<bool>;

    public class DeleteBillingHandler : IRequestHandler<DeleteBillingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBillingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteBillingCommand request, CancellationToken cancellationToken)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(request.Id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), request.Id);

            await _unitOfWork.Billings.DeleteAsync(billing);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
