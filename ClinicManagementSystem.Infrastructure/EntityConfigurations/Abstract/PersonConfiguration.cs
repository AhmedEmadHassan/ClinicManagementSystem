using ClinicManagementSystem.Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagementSystem.Infrastructure.EntityConfigurations.Abstract
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // 1. Primary Key
            builder.HasKey(p => p.Id);
            // 2. Properties Required, MaxLength, etc.
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.Phone)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(p => p.Gender)
                .IsRequired();
            builder.Property(p => p.Email)
                .HasMaxLength(255);
            builder.Property(p => p.Address)
                .HasMaxLength(100);
            builder.Property(p => p.Summary)
                .HasMaxLength(500);
            // 3. Relationships - Delete Behavior
            // 4. Indexes

            // 5. Default values

            // 6. Table name / schema
        }
    }
}
