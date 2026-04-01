using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Create;
using ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Delete;
using ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Update;
using ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetAll;
using ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AppointmentStateController : BaseController
    {
        private readonly IMediator _mediator;

        public AppointmentStateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAllAppointmentStatesQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Success(await _mediator.Send(new GetAppointmentStateByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentStateDTO dto)
            => Created(await _mediator.Send(new CreateAppointmentStateCommand(dto)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateAppointmentStateDTO dto)
            => Success(await _mediator.Send(new UpdateAppointmentStateCommand(id, dto)));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteAppointmentStateCommand(id));
            return NoContent();
        }
    }
}
