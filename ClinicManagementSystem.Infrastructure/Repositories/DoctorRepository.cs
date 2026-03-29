using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories.Bases;

namespace ClinicManagementSystem.Infrastructure.Repositories
{
    public class DoctorRepository : GenericRepositoryAsync<Doctor>, IDoctorRepository
    {
        private readonly AppDbContext _context;
        public DoctorRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
