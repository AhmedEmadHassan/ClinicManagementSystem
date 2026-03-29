using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.DTOs.Response
{
    public class AppointmentResponseDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentStateId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string AppointmentStateName { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
