using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Domain.Entities
{
    public class Doctor : Person
    {
        [ForeignKey(nameof(DoctorSpecialization))]
        public int DoctorSpecializationId { get; set; }
    }
}
