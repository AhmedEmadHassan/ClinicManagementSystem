using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories.Bases;

namespace ClinicManagementSystem.Infrastructure.Repositories
{
    public class SessionRepository : GenericRepositoryAsync<Session>, ISessionRepository
    {
        private readonly AppDbContext _context;
        public SessionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
