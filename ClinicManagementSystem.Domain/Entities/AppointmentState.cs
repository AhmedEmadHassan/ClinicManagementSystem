using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Domain.Entities
{
    public class AppointmentState
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
