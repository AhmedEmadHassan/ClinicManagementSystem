using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories.Bases;

namespace ClinicManagementSystem.Infrastructure.Repositories
{
    public class AppointmentStateRepository : GenericRepositoryAsync<AppointmentState>, IAppointmentStateRepository
    {
        private readonly AppDbContext _context;
        public AppointmentStateRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
