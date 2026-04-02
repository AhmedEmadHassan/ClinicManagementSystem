namespace ClinicManagementSystem.Application.DTOs.UpdateDTOs
{
    public class UpdateBillingDTO
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}
