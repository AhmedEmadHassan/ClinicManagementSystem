using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Domain.Entities
{
    public class Session
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Appointment))]
        public int AppointmentId { get; set; }
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }
        [MaxLength(500)]
        public string? ConsultationNotes { get; set; }
        [MaxLength(500)]
        public string? Prescriptions { get; set; }
    }
}
