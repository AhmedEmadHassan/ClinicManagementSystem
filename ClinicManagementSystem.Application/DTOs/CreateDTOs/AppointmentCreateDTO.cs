namespace ClinicManagementSystem.Application.DTOs.CreateDTOs
{
    public class AppointmentCreateDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentStateId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
