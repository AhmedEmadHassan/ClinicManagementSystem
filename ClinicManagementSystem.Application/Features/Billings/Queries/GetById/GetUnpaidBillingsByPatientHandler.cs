using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Queries.GetById
{
    // GetUnpaidBillingsByPatientQuery
    public record GetUnpaidBillingsByPatientQuery(int PatientId, PaginationRequest Pagination)
        : IRequest<PaginatedResponse<ResponseBillingDTO>>;

    public class GetUnpaidBillingsByPatientHandler
        : IRequestHandler<GetUnpaidBillingsByPatientQuery, PaginatedResponse<ResponseBillingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetUnpaidBillingsByPatientHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedResponse<ResponseBillingDTO>> Handle(
            GetUnpaidBillingsByPatientQuery request, CancellationToken cancellationToken)
        {
            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == request.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), request.PatientId);

            var cacheKey = $"Billing:Unpaid:{request.PatientId}:{request.Pagination.PageNumber}:{request.Pagination.PageSize}";
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
                },
                b => b.PatientId == request.PatientId && !b.IsPaid);

            _cache.Set(cacheKey, paged);
            return paged;
        }
    }
}
