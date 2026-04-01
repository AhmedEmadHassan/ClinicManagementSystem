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

        public GetAllDoctorsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResponse<ResponseDoctorDTO>> Handle(
            GetAllDoctorsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Doctors.GetPagedAsync(
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
        }
    }
}
