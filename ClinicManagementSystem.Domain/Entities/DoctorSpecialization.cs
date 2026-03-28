using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Domain.Entities
{
    public class DoctorSpecialization
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
