using ClinicManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.EntityConfigurations
{
    public class BillingConfigurations : IEntityTypeConfiguration<Billing>
    {
        public void Configure(EntityTypeBuilder<Billing> builder)
        {
            //// 1. Primary Key
            builder.HasKey(b => b.Id);
            //// 2. Properties Required, MaxLength, Precision,etc.
            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(b => b.Amount)
                .IsRequired();
            builder.Property(b => b.IsPaid)
                .IsRequired();
            builder.Property(b => b.Amount)
            .HasPrecision(18, 2);
            //// 3. Relationships - Delete Behavior
            builder.HasOne(b => b.Session)
                .WithMany(s => s.Billings)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Patient)
                .WithMany(p => p.Billings)
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            //// 4. Indexes

            //// 5. Default values

            //// 6. Table name / schema

        }
    }
}
