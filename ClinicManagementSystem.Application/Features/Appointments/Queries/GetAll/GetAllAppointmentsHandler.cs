using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Queries.GetAll
{
    public record GetAllAppointmentsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseAppointmentDTO>>;

    public class GetAllAppointmentsHandler
        : IRequestHandler<GetAllAppointmentsQuery, PaginatedResponse<ResponseAppointmentDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAppointmentsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResponse<ResponseAppointmentDTO>> Handle(
            GetAllAppointmentsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Appointments.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                a => new ResponseAppointmentDTO
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    AppointmentStateId = a.AppointmentStateId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    PatientName = a.Patient.Name,
                    DoctorName = a.Doctor.Name,
                    AppointmentStateName = a.AppointmentState.Name
                });
        }
    }
}
