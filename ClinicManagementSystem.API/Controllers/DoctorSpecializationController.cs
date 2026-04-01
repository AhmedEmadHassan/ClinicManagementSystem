using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Create;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Delete;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Update;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetAll;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DoctorSpecializationController : BaseController
    {
        private readonly IMediator _mediator;

        public DoctorSpecializationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAllDoctorSpecializationsQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Success(await _mediator.Send(new GetDoctorSpecializationByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDoctorSpecializationDTO dto)
            => Created(await _mediator.Send(new CreateDoctorSpecializationCommand(dto)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateDoctorSpecializationDTO dto)
            => Success(await _mediator.Send(new UpdateDoctorSpecializationCommand(id, dto)));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteDoctorSpecializationCommand(id));
            return NoContent();
        }
    }
}
