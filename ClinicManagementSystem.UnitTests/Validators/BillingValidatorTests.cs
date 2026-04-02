using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Features.Billings.Commands.Create;
using ClinicManagementSystem.Application.Features.Billings.Commands.Update;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class BillingValidatorTests
    {
        private readonly CreateBillingValidator _createValidator = new();
        private readonly UpdateBillingValidator _updateValidator = new();

        [Fact]
        public async Task Create_Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new CreateBillingCommand(new CreateBillingDTO
            {
                SessionId = 1,
                Description = "Consultation",
                Amount = 100
            });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Create_Validate_WhenSessionIdIsZero_ShouldFail()
        {
            var command = new CreateBillingCommand(new CreateBillingDTO
            {
                SessionId = 0,
                Description = "Consultation",
                Amount = 100
            });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "SessionId must be a valid id.");
        }

        [Fact]
        public async Task Create_Validate_WhenAmountIsZero_ShouldFail()
        {
            var command = new CreateBillingCommand(new CreateBillingDTO
            {
                SessionId = 1,
                Description = "Consultation",
                Amount = 0
            });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Amount must be greater than 0.");
        }

        [Fact]
        public async Task Create_Validate_WhenDescriptionIsEmpty_ShouldFail()
        {
            var command = new CreateBillingCommand(new CreateBillingDTO
            {
                SessionId = 1,
                Description = "",
                Amount = 100
            });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Description is required.");
        }

        [Fact]
        public async Task Update_Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new UpdateBillingCommand(1, new UpdateBillingDTO
            {
                Description = "Updated Consultation",
                Amount = 200
            });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Update_Validate_WhenAmountIsZero_ShouldFail()
        {
            var command = new UpdateBillingCommand(1, new UpdateBillingDTO
            {
                Description = "Consultation",
                Amount = 0
            });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Amount must be greater than 0.");
        }

        [Fact]
        public async Task Update_Validate_WhenDescriptionIsEmpty_ShouldFail()
        {
            var command = new UpdateBillingCommand(1, new UpdateBillingDTO
            {
                Description = "",
                Amount = 100
            });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Description is required.");
        }
    }
}
