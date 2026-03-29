using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories.Bases;

namespace ClinicManagementSystem.Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepositoryAsync<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
