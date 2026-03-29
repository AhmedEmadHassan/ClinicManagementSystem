using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.EntityConfigurations
{
    public class SessionConfigurations : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            // 1. Primary Key
            builder.HasKey(s => s.Id);
            // 2. Properties Required, MaxLength, etc.
            builder.Property(s => s.ConsultationNotes)
                .HasMaxLength(500);
            builder.Property(s => s.Prescriptions)
                .HasMaxLength(500);

            // 3. Relationships - Delete Behavior
            builder.HasOne(s => s.Appointment)
                .WithOne(a => a.Session)
                .HasForeignKey<Session>(s => s.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Patient)
                .WithMany(p => p.Sessions)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Doctor)
                .WithMany(d => d.Sessions)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Billings)
                .WithOne(b => b.Session)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.Restrict);


            // 4. Indexes

            // 5. Default values

            // 6. Table name / schema
        }
    }
}
