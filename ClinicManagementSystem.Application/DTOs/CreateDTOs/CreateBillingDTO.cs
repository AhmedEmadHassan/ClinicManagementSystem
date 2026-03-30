namespace ClinicManagementSystem.Application.DTOs.CreateDTOs
{
    public class CreateBillingDTO
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int PatientId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}
