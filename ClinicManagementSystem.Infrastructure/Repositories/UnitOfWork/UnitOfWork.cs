using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Application.UnitOfWork;
using ClinicManagementSystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClinicManagementSystem.Infrastructure.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(
            AppDbContext context,
            IAppointmentRepository appointmentRepo,
            IAppointmentStateRepository appointmentStateRepo,
            IBillingRepository billingRepo,
            IDoctorRepository doctorRepo,
            IDoctorSpecializationRepository doctorSpecRepo,
            IPatientRepository patientRepo,
            ISessionRepository sessionRepo)
        {
            _context = context;
            Appointments = appointmentRepo;
            AppointmentStates = appointmentStateRepo;
            Billings = billingRepo;
            Doctors = doctorRepo;
            DoctorSpecializations = doctorSpecRepo;
            Patients = patientRepo;
            Sessions = sessionRepo;
        }

        public IAppointmentRepository Appointments { get; }
        public IAppointmentStateRepository AppointmentStates { get; }
        public IBillingRepository Billings { get; }
        public IDoctorRepository Doctors { get; }
        public IDoctorSpecializationRepository DoctorSpecializations { get; }
        public IPatientRepository Patients { get; }
        public ISessionRepository Sessions { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
                _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }

}
