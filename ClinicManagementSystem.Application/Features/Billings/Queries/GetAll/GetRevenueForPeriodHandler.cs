using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using FluentValidation;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Billings.Queries.GetAll
{
    // GetRevenueForPeriodQuery
    public record GetRevenueForPeriodQuery(DateOnly StartDate, DateOnly EndDate) : IRequest<RevenueResponseDTO>;

    public class RevenueResponseDTO
    {
        public decimal TotalRevenue { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalPaidBillings { get; set; }
    }

    public class GetRevenueForPeriodHandler : IRequestHandler<GetRevenueForPeriodQuery, RevenueResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRevenueForPeriodHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RevenueResponseDTO> Handle(
            GetRevenueForPeriodQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate)
                throw new BadRequestException("StartDate must be before or equal to EndDate.");

            var billings = await _unitOfWork.Billings.GetAllAsync(
                b => new { b.Amount, b.IsPaid, AppointmentDate = b.Session.Appointment.AppointmentDate },
                b => b.IsPaid &&
                     b.Session.Appointment.AppointmentDate >= request.StartDate &&
                     b.Session.Appointment.AppointmentDate <= request.EndDate);

            return new RevenueResponseDTO
            {
                TotalRevenue = billings.Sum(b => b.Amount),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPaidBillings = billings.Count
            };
        }
    }

    public class GetRevenueForPeriodValidator : AbstractValidator<GetRevenueForPeriodQuery>
    {
        public GetRevenueForPeriodValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
        }
    }
}
