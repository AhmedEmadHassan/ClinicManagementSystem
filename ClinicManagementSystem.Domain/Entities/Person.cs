using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Domain.Entities
{
    public abstract class Person
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public bool Gender { get; set; }
        [MaxLength(255)]
        public string? Email { get; set; }
        [MaxLength(100)]
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        [MaxLength(50)]
        public string? Summary { get; set; }
    }
}
