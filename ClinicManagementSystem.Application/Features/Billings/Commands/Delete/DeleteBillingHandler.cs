using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Commands.Delete
{
    // Commands/Delete
    public record DeleteBillingCommand(int Id) : IRequest<bool>;

    public class DeleteBillingHandler
    : IRequestHandler<DeleteBillingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public DeleteBillingHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<bool> Handle(
            DeleteBillingCommand request, CancellationToken cancellationToken)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(request.Id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), request.Id);

            await _unitOfWork.Billings.DeleteAsync(billing);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.Billing);

            return true;
        }
    }
}
