using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Billings.Commands.Create;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class BillingValidatorTests
    {
        private readonly CreateBillingValidator _validator = new();

        [Fact]
        public async Task Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new CreateBillingCommand(new CreateBillingDTO
            {
                SessionId = 1,
                PatientId = 1,
                Description = "Consultation",
                Amount = 100
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WhenAmountIsZero_ShouldFail()
        {
            var command = new CreateBillingCommand(new CreateBillingDTO
            {
                SessionId = 1,
                PatientId = 1,
                Description = "Consultation",
                Amount = 0
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Amount must be greater than 0.");
        }

        [Fact]
        public async Task Validate_WhenDescriptionIsEmpty_ShouldFail()
        {
            var command = new CreateBillingCommand(new CreateBillingDTO
            {
                SessionId = 1,
                PatientId = 1,
                Description = "",
                Amount = 100
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Description is required.");
        }
    }
}
