using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories.Bases;

namespace ClinicManagementSystem.Infrastructure.Repositories
{
    public class DoctorSpecializationRepository : GenericRepositoryAsync<DoctorSpecialization>, IDoctorSpecializationRepository
    {
        private readonly AppDbContext _context;
        public DoctorSpecializationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
