using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Dashboard.Queries
{
    // DashboardResponseDTO
    public class DashboardResponseDTO
    {
        public int TodayAppointmentsCount { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public int PendingUnpaidBillingsCount { get; set; }
        public List<MostActiveDoctorDTO> MostActiveDoctors { get; set; } = new();
    }

    public class MostActiveDoctorDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int SessionCount { get; set; }
    }
    // GetDashboardQuery
    public record GetDashboardQuery : IRequest<DashboardResponseDTO>;

    public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, DashboardResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetDashboardHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<DashboardResponseDTO> Handle(
            GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Dashboard:{DateOnly.FromDateTime(DateTime.UtcNow)}";
            var cached = _cache.Get<DashboardResponseDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);

            // Today's appointments count
            var todayCount = await _unitOfWork.Appointments.AnyAsync(a => a.AppointmentDate == today)
                ? _unitOfWork.Appointments.GetTableNoTracking().Count(a => a.AppointmentDate == today)
                : 0;

            // Total revenue this month
            var monthlyRevenue = _unitOfWork.Billings
                .GetTableNoTracking()
                .Where(b => b.IsPaid &&
                            b.Session.Appointment.AppointmentDate >= firstDayOfMonth &&
                            b.Session.Appointment.AppointmentDate <= today)
                .Sum(b => (decimal?)b.Amount) ?? 0;

            // Pending unpaid billings count
            var unpaidCount = _unitOfWork.Billings
                .GetTableNoTracking()
                .Count(b => !b.IsPaid);

            // Most active doctors (top 5)
            var mostActiveDoctors = _unitOfWork.Sessions
                .GetTableNoTracking()
                .GroupBy(s => new { s.DoctorId, s.Doctor.Name })
                .Select(g => new MostActiveDoctorDTO
                {
                    DoctorId = g.Key.DoctorId,
                    DoctorName = g.Key.Name,
                    SessionCount = g.Count()
                })
                .OrderByDescending(d => d.SessionCount)
                .Take(5)
                .ToList();

            var result = new DashboardResponseDTO
            {
                TodayAppointmentsCount = todayCount,
                TotalRevenueThisMonth = monthlyRevenue,
                PendingUnpaidBillingsCount = unpaidCount,
                MostActiveDoctors = mostActiveDoctors
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
    }
}
