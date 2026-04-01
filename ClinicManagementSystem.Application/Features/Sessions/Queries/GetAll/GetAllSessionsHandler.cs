using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Queries.GetAll
{
    public record GetAllSessionsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseSessionDTO>>;

    public class GetAllSessionsHandler
        : IRequestHandler<GetAllSessionsQuery, PaginatedResponse<ResponseSessionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSessionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResponse<ResponseSessionDTO>> Handle(
            GetAllSessionsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Sessions.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                s => new ResponseSessionDTO
                {
                    Id = s.Id,
                    AppointmentId = s.AppointmentId,
                    PatientId = s.PatientId,
                    DoctorId = s.DoctorId,
                    ConsultationNotes = s.ConsultationNotes,
                    Prescriptions = s.Prescriptions,
                    PatientName = s.Patient.Name,
                    DoctorName = s.Doctor.Name
                });
        }
    }
}
