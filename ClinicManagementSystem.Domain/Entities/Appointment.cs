using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Domain.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }
        [ForeignKey(nameof(AppointmentState))]
        public int AppointmentStateId { get; set; }
        [Required]
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }
        public string? Notes { get; set; }
    }
}
