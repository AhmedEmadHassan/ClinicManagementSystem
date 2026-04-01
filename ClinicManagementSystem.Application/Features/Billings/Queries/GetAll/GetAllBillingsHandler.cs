using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Queries.GetAll
{
    // Queries/GetAll
    public record GetAllBillingsQuery : IRequest<List<ResponseBillingDTO>>;

    public class GetAllBillingsHandler : IRequestHandler<GetAllBillingsQuery, List<ResponseBillingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBillingsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseBillingDTO>> Handle(GetAllBillingsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Billings.GetAllAsync(b => new ResponseBillingDTO
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
