namespace ClinicManagementSystem.Application.DTOs.ResponseDTOs
{
    public class SessionResponseDTO
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string? ConsultationNotes { get; set; }
        public string? Prescriptions { get; set; }

        public string DoctorName { get; set; }
        public string PatientName { get; set; }

    }
}
