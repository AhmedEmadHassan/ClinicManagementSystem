using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Billings.Commands.Create;
using ClinicManagementSystem.Application.Features.Billings.Commands.Delete;
using ClinicManagementSystem.Application.Features.Billings.Commands.Update;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Receptionist")]
    public class BillingController : BaseController
    {
        private readonly IMediator _mediator;

        public BillingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Success(await _mediator.Send(new GetAllBillingsQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Success(await _mediator.Send(new GetBillingByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBillingDTO dto)
            => Created(await _mediator.Send(new CreateBillingCommand(dto)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBillingDTO dto)
            => Success(await _mediator.Send(new UpdateBillingCommand(id, dto)));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteBillingCommand(id));
            return NoContent();
        }
    }
}
