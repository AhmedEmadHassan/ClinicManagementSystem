using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories.Bases;

namespace ClinicManagementSystem.Infrastructure.Repositories
{
    public class PatientRepository : GenericRepositoryAsync<Patient>, IPatientRepository
    {
        private readonly AppDbContext _context;
        public PatientRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
