using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Create;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class DoctorSpecializationValidatorTests
    {
        private readonly CreateDoctorSpecializationValidator _validator = new();

        [Fact]
        public async Task Validate_WhenNameIsValid_ShouldPass()
        {
            var command = new CreateDoctorSpecializationCommand(new CreateDoctorSpecializationDTO { Name = "Cardiology" });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WhenNameIsEmpty_ShouldFail()
        {
            var command = new CreateDoctorSpecializationCommand(new CreateDoctorSpecializationDTO { Name = "" });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Name is required.");
        }

        [Fact]
        public async Task Validate_WhenNameExceedsMaxLength_ShouldFail()
        {
            var command = new CreateDoctorSpecializationCommand(new CreateDoctorSpecializationDTO { Name = new string('A', 101) });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Name must not exceed 100 characters.");
        }
    }
}
