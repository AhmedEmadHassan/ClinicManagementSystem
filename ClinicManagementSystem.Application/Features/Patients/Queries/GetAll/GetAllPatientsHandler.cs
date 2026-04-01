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

        public GetAllPatientsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResponse<ResponsePatientDTO>> Handle(
            GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Patients.GetPagedAsync(
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
        }
    }
}
