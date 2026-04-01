using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Create;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Delete;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Update;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DoctorController : BaseController
    {
        private readonly IMediator _mediator;

        public DoctorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAllDoctorsQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Success(await _mediator.Send(new GetDoctorByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDTO dto)
            => Created(await _mediator.Send(new CreateDoctorCommand(dto)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateDoctorDTO dto)
            => Success(await _mediator.Send(new UpdateDoctorCommand(id, dto)));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteDoctorCommand(id));
            return NoContent();
        }
    }

}
