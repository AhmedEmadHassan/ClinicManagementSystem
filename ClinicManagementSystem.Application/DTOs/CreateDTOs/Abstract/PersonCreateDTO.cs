namespace ClinicManagementSystem.Application.DTOs.CreateDTOs.Abstract
{
    public abstract class PersonCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Summary { get; set; }
    }
}
