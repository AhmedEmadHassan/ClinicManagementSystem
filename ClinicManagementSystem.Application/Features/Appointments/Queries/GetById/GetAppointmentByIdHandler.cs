using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Appointments.Queries.GetById
{
    // Queries/GetById
    public record GetAppointmentByIdQuery(int Id) : IRequest<ResponseAppointmentDTO>;

    public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdQuery, ResponseAppointmentDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAppointmentByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseAppointmentDTO> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id);

            if (appointment is null)
                throw new NotFoundException(nameof(Appointment), request.Id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(appointment.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId);
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(appointment.AppointmentStateId);

            appointment.Patient = patient;
            appointment.Doctor = doctor;
            appointment.AppointmentState = state;

            return _mapper.Map<ResponseAppointmentDTO>(appointment);
        }
    }
}
