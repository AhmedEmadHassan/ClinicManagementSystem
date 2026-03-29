using ClinicManagementSystem.Domain.Entities.Abstract;

namespace ClinicManagementSystem.Domain.Entities
{
    public class Doctor : Person
    {
        public int DoctorSpecializationId { get; set; }
        public DoctorSpecialization DoctorSpecialization { get; set; }

        public ICollection<Session> Sessions { get; set; }
        public ICollection<Appointment> Appointments { get; set; }

    }
}
