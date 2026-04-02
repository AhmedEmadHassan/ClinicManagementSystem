namespace ClinicManagementSystem.Application.DTOs.CreateDTOs
{
    public class CreateSessionDTO
    {
        public int AppointmentId { get; set; }
        public string? ConsultationNotes { get; set; }
        public string? Prescriptions { get; set; }
    }
}
