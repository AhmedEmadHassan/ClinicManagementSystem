using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Patients.Commands.Create;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class PatientValidatorTests
    {
        private readonly CreatePatientValidator _validator = new();

        [Fact]
        public async Task Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new CreatePatientCommand(new CreatePatientDTO
            {
                Name = "John",
                Phone = "123456789",
                Gender = "Male"
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WhenNameIsEmpty_ShouldFail()
        {
            var command = new CreatePatientCommand(new CreatePatientDTO { Name = "", Phone = "123", Gender = "Male" });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Name is required.");
        }

        [Fact]
        public async Task Validate_WhenGenderIsInvalid_ShouldFail()
        {
            var command = new CreatePatientCommand(new CreatePatientDTO { Name = "John", Phone = "123", Gender = "Unknown" });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Gender must be 'Male' or 'Female'.");
        }

        [Fact]
        public async Task Validate_WhenEmailIsInvalid_ShouldFail()
        {
            var command = new CreatePatientCommand(new CreatePatientDTO { Name = "John", Phone = "123", Gender = "Male", Email = "invalid-email" });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Invalid email format.");
        }

        [Fact]
        public async Task Validate_WhenEmailIsEmpty_ShouldPass()
        {
            var command = new CreatePatientCommand(new CreatePatientDTO { Name = "John", Phone = "123", Gender = "Male", Email = null });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }
    }
}
