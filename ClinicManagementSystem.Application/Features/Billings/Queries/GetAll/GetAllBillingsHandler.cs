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

        public GetAllBillingsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResponse<ResponseBillingDTO>> Handle(
            GetAllBillingsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Billings.GetPagedAsync(
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
        }
    }
}
