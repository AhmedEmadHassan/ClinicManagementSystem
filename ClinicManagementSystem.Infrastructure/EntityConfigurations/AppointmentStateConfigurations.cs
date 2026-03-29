using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.EntityConfigurations
{
    public class AppointmentStateConfigurations : IEntityTypeConfiguration<AppointmentState>
    {
        public void Configure(EntityTypeBuilder<AppointmentState> builder)
        {
            //// 1. Primary Key
            builder.HasKey(s => s.Id);
            //// 2. Properties Required, MaxLength, etc.
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);
            //// 3. Relationships - Delete Behavior
            builder.HasMany(s => s.Appointments)
                .WithOne(a => a.AppointmentState)
                .HasForeignKey(a => a.AppointmentStateId)
                .OnDelete(DeleteBehavior.Restrict);
            //// 4. Indexes

            //// 5. Default values

            //// 6. Table name / schema

        }
    }
}
