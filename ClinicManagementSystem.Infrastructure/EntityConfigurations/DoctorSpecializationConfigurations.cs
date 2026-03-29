using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.EntityConfigurations
{
    public class DoctorSpecializationConfigurations : IEntityTypeConfiguration<DoctorSpecialization>
    {
        public void Configure(EntityTypeBuilder<DoctorSpecialization> builder)
        {
            // 1. Primary Key
            builder.HasKey(ds => ds.Id);
            // 2. Properties Required, MaxLength, etc.
            builder.Property(ds => ds.Name)
                .IsRequired()
                .HasMaxLength(50);
            // 3. Relationships - Delete Behavior
            builder.HasMany(ds => ds.Doctors)
                .WithOne(d => d.DoctorSpecialization)
                .HasForeignKey(d => d.DoctorSpecializationId)
                .OnDelete(DeleteBehavior.Restrict);
            // 4. Indexes

            // 5. Default values

            // 6. Table name / schema
        }
    }
}
