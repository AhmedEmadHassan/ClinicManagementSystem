namespace ClinicManagementSystem.Application.DTOs.ResponseDTOs
{
    public class BillingResponseDTO
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}
