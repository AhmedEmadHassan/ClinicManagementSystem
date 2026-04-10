using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Queries.GetAll
{
    public record GetAllBillingsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseBillingDTO>>;

    public class GetAllBillingsHandler
    : IRequestHandler<GetAllBillingsQuery, PaginatedResponse<ResponseBillingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetAllBillingsHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseBillingDTO>> Handle(
            GetAllBillingsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetAll(CacheKeys.Billing) + $":{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
            var cached = _cache.Get<PaginatedResponse<ResponseBillingDTO>>(cacheKey);

            if (cached is not null)
                return cached;

            var paged = await _unitOfWork.Billings.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                b => new ResponseBillingDTO
                {
                    Id = b.Id,
                    SessionId = b.SessionId,
                    PatientId = b.PatientId,
                    Description = b.Description,
                    Amount = b.Amount,
                    IsPaid = b.IsPaid,
                    PatientName = b.Patient.Name
                });

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
