namespace ClinicManagementSystem.Application.DTOs.CreateDTOs.Abstract
{
    public abstract class CreatePersonDTO
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; } // Changed from bool to string
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Summary { get; set; }
    }
}
