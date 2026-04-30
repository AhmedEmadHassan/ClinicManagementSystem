using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Receptionist,Doctor,Patient")]
    public class DoctorUseCasesController : BaseController
    {
        private readonly IMediator _mediator;

        public DoctorUseCasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("specialization/{specializationId}")]
        public async Task<IActionResult> GetBySpecialization(int specializationId, [FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetDoctorsBySpecializationQuery(specializationId, pagination)));

        [HttpGet("{doctorId}/availability")]
        public async Task<IActionResult> GetAvailability(int doctorId, [FromQuery] DateOnly date)
            => Success(await _mediator.Send(new GetDoctorAvailabilityQuery(doctorId, date)));
    }
}
