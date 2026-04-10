using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Delete
{
    // Commands/Delete
    public record DeleteDoctorSpecializationCommand(int Id) : IRequest<bool>;

    public class DeleteDoctorSpecializationHandler
    : IRequestHandler<DeleteDoctorSpecializationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public DeleteDoctorSpecializationHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<bool> Handle(
            DeleteDoctorSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(request.Id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), request.Id);

            await _unitOfWork.DoctorSpecializations.DeleteAsync(specialization);
            await _unitOfWork.SaveChangesAsync();

            _cache.RemoveByPrefix(CacheKeys.DoctorSpecialization);

            return true;
        }
    }
}
