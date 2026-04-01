using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Queries.GetAll
{
    // Queries/GetAll
    public record GetAllSessionsQuery : IRequest<List<ResponseSessionDTO>>;

    public class GetAllSessionsHandler : IRequestHandler<GetAllSessionsQuery, List<ResponseSessionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSessionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseSessionDTO>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Sessions.GetAllAsync(s => new ResponseSessionDTO
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
