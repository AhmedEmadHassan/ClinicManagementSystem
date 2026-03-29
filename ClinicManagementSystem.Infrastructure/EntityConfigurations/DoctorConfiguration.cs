using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.EntityConfigurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            // 1. Primary Key
            //builder.HasKey(d => d.Id);
            // 2. Properties Required, MaxLength, etc.
            builder.Property(d => d.DoctorSpecializationId)
                            .IsRequired();

            // 3. Relationships - Delete Behavior
            builder.HasOne(d => d.DoctorSpecialization)
                .WithMany(ds => ds.Doctors)
                .HasForeignKey(d => d.DoctorSpecializationId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(d => d.Sessions)
                .WithOne(s => s.Doctor)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            // 4. Indexes

            // 5. Default values

            // 6. Table name / schema
        }
    }
}
