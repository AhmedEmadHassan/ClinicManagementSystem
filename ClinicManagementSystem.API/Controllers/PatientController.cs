using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Patients.Commands.Create;
using ClinicManagementSystem.Application.Features.Patients.Commands.Delete;
using ClinicManagementSystem.Application.Features.Patients.Commands.Update;
using ClinicManagementSystem.Application.Features.Patients.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Patients.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Receptionist")]
    public class PatientController : BaseController
    {
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetAllPatientsQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Success(await _mediator.Send(new GetPatientByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePatientDTO dto)
            => Created(await _mediator.Send(new CreatePatientCommand(dto)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePatientDTO dto)
            => Success(await _mediator.Send(new UpdatePatientCommand(id, dto)));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeletePatientCommand(id));
            return NoContent();
        }
    }
}
