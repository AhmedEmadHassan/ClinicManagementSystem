using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Doctors.Queries.GetAll
{
    // GetDoctorsBySpecializationQuery
    public record GetDoctorsBySpecializationQuery(int SpecializationId, PaginationRequest Pagination)
        : IRequest<PaginatedResponse<ResponseDoctorDTO>>;

    public class GetDoctorsBySpecializationHandler
        : IRequestHandler<GetDoctorsBySpecializationQuery, PaginatedResponse<ResponseDoctorDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetDoctorsBySpecializationHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseDoctorDTO>> Handle(
            GetDoctorsBySpecializationQuery request, CancellationToken cancellationToken)
        {
            var specializationExists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Id == request.SpecializationId);
            if (!specializationExists)
                throw new NotFoundException(nameof(DoctorSpecialization), request.SpecializationId);

            var cacheKey = $"Doctor:BySpecialization:{request.SpecializationId}:{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
            var cached = _cache.Get<PaginatedResponse<ResponseDoctorDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var paged = await _unitOfWork.Doctors.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                d => new ResponseDoctorDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Phone = d.Phone,
                    Gender = d.Gender ? "Male" : "Female",
                    Email = d.Email,
                    Address = d.Address,
                    DateOfBirth = d.DateOfBirth,
                    Summary = d.Summary,
                    DoctorSpecializationId = d.DoctorSpecializationId,
                    DoctorSpecializationName = d.DoctorSpecialization.Name
                },
                d => d.DoctorSpecializationId == request.SpecializationId);

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
