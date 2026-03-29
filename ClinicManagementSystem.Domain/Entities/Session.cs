namespace ClinicManagementSystem.Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string? ConsultationNotes { get; set; }
        public string? Prescriptions { get; set; }

        public Appointment Appointment { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

        public ICollection<Billing> Billings { get; set; }
    }
}
