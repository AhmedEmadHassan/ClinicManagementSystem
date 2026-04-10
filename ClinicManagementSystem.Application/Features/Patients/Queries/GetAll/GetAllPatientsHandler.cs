using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Patients.Queries.GetAll
{
    public record GetAllPatientsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponsePatientDTO>>;

    public class GetAllPatientsHandler
    : IRequestHandler<GetAllPatientsQuery, PaginatedResponse<ResponsePatientDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetAllPatientsHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponsePatientDTO>> Handle(
            GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetAll(CacheKeys.Patient) + $":{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
            var cached = _cache.Get<PaginatedResponse<ResponsePatientDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var paged = await _unitOfWork.Patients.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                p => new ResponsePatientDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Phone = p.Phone,
                    Gender = p.Gender ? "Male" : "Female",
                    Email = p.Email,
                    Address = p.Address,
                    DateOfBirth = p.DateOfBirth,
                    Summary = p.Summary
                });

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
