using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Domain.Entities
{
    public class Billing
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Session))]
        public int SessionId { get; set; }
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public bool IsPaid { get; set; }
    }
}
