using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
