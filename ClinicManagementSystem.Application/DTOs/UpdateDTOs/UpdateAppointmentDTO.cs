namespace ClinicManagementSystem.Application.DTOs.UpdateDTOs
{
    public class UpdateAppointmentDTO
    {
        public int AppointmentStateId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly? AppointmentTime { get; set; }
        public string? Notes { get; set; }
    }
}
