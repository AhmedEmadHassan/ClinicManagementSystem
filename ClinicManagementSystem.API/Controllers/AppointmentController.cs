using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Create;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Delete;
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
    public class AppointmentController : BaseController
    {
        private readonly IMediator _mediator;

        public AppointmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAllAppointmentsQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Success(await _mediator.Send(new GetAppointmentByIdQuery(id)));

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO dto)
            => Created(await _mediator.Send(new CreateAppointmentCommand(dto)));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDTO dto)
            => Success(await _mediator.Send(new UpdateAppointmentCommand(id, dto)));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteAppointmentCommand(id));
            return NoContent();
        }
    }
}
