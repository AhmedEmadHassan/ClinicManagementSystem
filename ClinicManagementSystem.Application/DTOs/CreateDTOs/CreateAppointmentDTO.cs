namespace ClinicManagementSystem.Application.DTOs.CreateDTOs
{
    public class CreateAppointmentDTO
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentStateId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }
        public string? Notes { get; set; }
    }
}
