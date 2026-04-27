using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Update;
using ClinicManagementSystem.Application.Features.Appointments.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Appointments.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Receptionist,Doctor,Patient")]
    public class AppointmentUseCasesController : BaseController
    {
        private readonly IMediator _mediator;

        public AppointmentUseCasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctor(int doctorId, [FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAppointmentsByDoctorQuery(doctorId, pagination)));

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId, [FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAppointmentsByPatientQuery(patientId, pagination)));

        [HttpGet("today")]
        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> GetToday([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetTodaysAppointmentsQuery(pagination)));

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Cancel(int id)
            => Success(await _mediator.Send(new CancelAppointmentCommand(id)));

        [HttpPut("{id}/reschedule")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleAppointmentDTO dto)
            => Success(await _mediator.Send(new RescheduleAppointmentCommand(id, dto)));
    }
}
