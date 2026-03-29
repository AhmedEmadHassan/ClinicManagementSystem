using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentState> AppointmentStates { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
