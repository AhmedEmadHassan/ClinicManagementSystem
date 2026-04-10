using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Doctors.Queries.GetAll
{
    public record GetAllDoctorsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseDoctorDTO>>;

    public class GetAllDoctorsHandler
    : IRequestHandler<GetAllDoctorsQuery, PaginatedResponse<ResponseDoctorDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetAllDoctorsHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseDoctorDTO>> Handle(
            GetAllDoctorsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetAll(CacheKeys.Doctor) + $":{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
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
                });

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
