using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : BaseController
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
            => Success(await _mediator.Send(new GetDashboardQuery()));
    }
}
