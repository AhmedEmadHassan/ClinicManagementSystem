using ClinicManagementSystem.Domain.Entities.Abstract;

namespace ClinicManagementSystem.Domain.Entities
{
    public class Patient : Person
    {
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Billing> Billings { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
