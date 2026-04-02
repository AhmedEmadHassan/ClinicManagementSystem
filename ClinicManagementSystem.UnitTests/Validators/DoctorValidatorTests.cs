using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Create;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class DoctorValidatorTests
    {
        private readonly CreateDoctorValidator _validator = new();

        [Fact]
        public async Task Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new CreateDoctorCommand(new CreateDoctorDTO
            {
                Name = "Dr.Smith",
                Phone = "123456789",
                Gender = "Male",
                DoctorSpecializationId = 1
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WhenSpecializationIdIsZero_ShouldFail()
        {
            var command = new CreateDoctorCommand(new CreateDoctorDTO
            {
                Name = "Dr.Smith",
                Phone = "123",
                Gender = "Male",
                DoctorSpecializationId = 0
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "DoctorSpecializationId must be a valid id.");
        }

        [Fact]
        public async Task Validate_WhenGenderIsInvalid_ShouldFail()
        {
            var command = new CreateDoctorCommand(new CreateDoctorDTO
            {
                Name = "Dr.Smith",
                Phone = "123",
                Gender = "Other",
                DoctorSpecializationId = 1
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Gender must be 'Male' or 'Female'.");
        }
    }
}
