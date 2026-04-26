using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Features.Users.Commands.Delete;
using ClinicManagementSystem.Application.Features.Users.Commands.Update;
using ClinicManagementSystem.Application.Features.Users.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAllUsersQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
            => Success(await _mediator.Send(new GetUserByIdQuery(id)));

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetByRole(string role)
            => Success(await _mediator.Send(new GetUsersByRoleQuery(role)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDTO dto)
            => Success(await _mediator.Send(new UpdateUserCommand(id, dto)));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return NoContent();
        }

        [HttpPut("{id}/change-role")]
        public async Task<IActionResult> ChangeRole(string id, [FromBody] ChangeUserRoleDTO dto)
            => Success(await _mediator.Send(new ChangeUserRoleCommand(id, dto)));
    }
}
