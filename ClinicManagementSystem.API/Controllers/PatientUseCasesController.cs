using Asp.Versioning;
using ClinicManagementSystem.API.Controllers.Base;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.Features.Patients.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Receptionist,Doctor,Patient")]
    public class PatientUseCasesController : BaseController
    {
        private readonly IMediator _mediator;

        public PatientUseCasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{patientId}/medical-history")]
        public async Task<IActionResult> GetMedicalHistory(int patientId)
            => Success(await _mediator.Send(new GetPatientMedicalHistoryQuery(patientId)));

        [HttpGet("{patientId}/active-appointments")]
        public async Task<IActionResult> GetActiveAppointments(int patientId, [FromQuery] PaginationRequest pagination)
            => Success(await _mediator.Send(new GetPatientActiveAppointmentsQuery(patientId, pagination)));
    }
}
