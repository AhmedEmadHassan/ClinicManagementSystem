namespace ClinicManagementSystem.Application.DTOs.ResponseDTOs
{
    public class ResponseAppointmentDTO
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
    }
}
