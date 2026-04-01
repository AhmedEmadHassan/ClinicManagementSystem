using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Create;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Delete;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Update;
using ClinicManagementSystem.Application.Features.Sessions.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Sessions.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Doctor")]
    public class SessionController : BaseController
    {
        private readonly IMediator _mediator;

        public SessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAllSessionsQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Success(await _mediator.Send(new GetSessionByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSessionDTO dto)
            => Created(await _mediator.Send(new CreateSessionCommand(dto)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateSessionDTO dto)
            => Success(await _mediator.Send(new UpdateSessionCommand(id, dto)));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteSessionCommand(id));
            return NoContent();
        }
    }
}
