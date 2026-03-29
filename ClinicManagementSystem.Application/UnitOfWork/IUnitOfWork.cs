using ClinicManagementSystem.Application.RepositoryInterfaces;

namespace ClinicManagementSystem.Application.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IAppointmentRepository Appointments { get; }
        IAppointmentStateRepository AppointmentStates { get; }
        IBillingRepository Billings { get; }
        IDoctorRepository Doctors { get; }
        IDoctorSpecializationRepository DoctorSpecializations { get; }
        IPatientRepository Patients { get; }
        ISessionRepository Sessions { get; }

        // Save all changes
        Task<int> SaveChangesAsync();

        // Transaction control
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}
