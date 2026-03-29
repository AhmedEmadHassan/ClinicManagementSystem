using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.EntityConfigurations
{
    public class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            //// 1. Primary Key
            builder.HasKey(a => a.Id);
            //// 2. Properties Required, MaxLength, ForignKeyRequired if Mandatory only, etc.
            builder.Property(a => a.AppointmentDate).IsRequired();
            builder.Property(a => a.Notes).HasMaxLength(500);
            builder.Property(a => a.AppointmentStateId).IsRequired();
            //// 3. Relationships - Delete Behavior
            // PatientId
            builder.HasOne(a => a.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);
            // DoctorId
            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            // AppointmentStateId
            builder.HasOne(a => a.AppointmentState)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.AppointmentStateId)
                .OnDelete(DeleteBehavior.Restrict);
            //Many Sessions
            builder.HasOne(a => a.Session)
                .WithOne(s => s.Appointment)
                .HasForeignKey<Session>(s => s.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            //// 4. Indexes
            builder.HasIndex(a => a.PatientId);

            //// 5. Default values
            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            //// 6. Table name / schema
            //builder.ToTable("Appointments", "dbo");

        }
    }
}
