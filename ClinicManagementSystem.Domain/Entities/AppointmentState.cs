namespace ClinicManagementSystem.Domain.Entities
{
    public class AppointmentState
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Appointment> Appointments { get; set; }
    }
}
