using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories.Bases;

namespace ClinicManagementSystem.Infrastructure.Repositories
{
    public class BillingRepository : GenericRepositoryAsync<Billing>, IBillingRepository
    {
        private readonly AppDbContext _context;
        public BillingRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
