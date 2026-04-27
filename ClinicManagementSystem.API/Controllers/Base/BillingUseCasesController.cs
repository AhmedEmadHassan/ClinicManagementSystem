using Asp.Versioning;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.Features.Billings.Commands.Update;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers.Base
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Receptionist")]
    public class BillingUseCasesController : BaseController
    {
        private readonly IMediator _mediator;

        public BillingUseCasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("unpaid/patient/{patientId}")]
        public async Task<IActionResult> GetUnpaidByPatient(int patientId, [FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetUnpaidBillingsByPatientQuery(patientId, pagination)));

        [HttpPut("{id}/mark-paid")]
        public async Task<IActionResult> MarkAsPaid(int id)
            => Success(await _mediator.Send(new MarkBillingAsPaidCommand(id)));

        [HttpGet("revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenue([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
            => Success(await _mediator.Send(new GetRevenueForPeriodQuery(startDate, endDate)));
    }
}
