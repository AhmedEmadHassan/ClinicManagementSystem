using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Doctors.Queries.GetById
{
    // GetDoctorAvailabilityQuery
    public record GetDoctorAvailabilityQuery(int DoctorId, DateOnly Date)
        : IRequest<DoctorAvailabilityResponseDTO>;

    public class DoctorAvailabilityResponseDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public List<TimeOnly> BookedSlots { get; set; } = new();
    }

    public class GetDoctorAvailabilityHandler
        : IRequestHandler<GetDoctorAvailabilityQuery, DoctorAvailabilityResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDoctorAvailabilityHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorAvailabilityResponseDTO> Handle(
            GetDoctorAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId);
            if (doctor is null)
                throw new NotFoundException(nameof(Doctor), request.DoctorId);

            var bookedSlots = await _unitOfWork.Appointments.GetAllAsync(
                a => a.AppointmentTime,
                a => a.DoctorId == request.DoctorId &&
                     a.AppointmentDate == request.Date &&
                     a.AppointmentTime != null);

            return new DoctorAvailabilityResponseDTO
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.Name,
                Date = request.Date,
                BookedSlots = bookedSlots.Where(t => t.HasValue).Select(t => t!.Value).ToList()
            };
        }
    }
}
