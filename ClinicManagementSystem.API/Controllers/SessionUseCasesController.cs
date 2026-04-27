using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Update;
using ClinicManagementSystem.Application.Features.Sessions.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Doctor")]
    public class SessionUseCasesController : BaseController
    {
        private readonly IMediator _mediator;

        public SessionUseCasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId, [FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetSessionsByPatientQuery(patientId, pagination)));

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctor(int doctorId, [FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetSessionsByDoctorQuery(doctorId, pagination)));

        [HttpPost("start/{appointmentId}")]
        public async Task<IActionResult> StartFromAppointment(int appointmentId)
            => Created(await _mediator.Send(new StartSessionFromAppointmentCommand(appointmentId)));
    }
}
