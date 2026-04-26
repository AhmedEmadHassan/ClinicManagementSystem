using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Features.Users.Commands.Update;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class UserValidatorTests
    {
        private readonly UpdateUserValidator _updateValidator = new();
        private readonly ChangeUserRoleValidator _changeRoleValidator = new();

        [Fact]
        public async Task UpdateUser_Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new UpdateUserCommand("1", new UpdateUserDTO { UserName = "newname", Email = "new@clinic.com" });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUser_Validate_WhenUserNameIsEmpty_ShouldFail()
        {
            var command = new UpdateUserCommand("1", new UpdateUserDTO { UserName = "", Email = "new@clinic.com" });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "UserName is required.");
        }

        [Fact]
        public async Task UpdateUser_Validate_WhenEmailIsInvalid_ShouldFail()
        {
            var command = new UpdateUserCommand("1", new UpdateUserDTO { UserName = "newname", Email = "invalid-email" });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Invalid email format.");
        }

        [Fact]
        public async Task ChangeRole_Validate_WhenRoleIsValid_ShouldPass()
        {
            var command = new ChangeUserRoleCommand("1", new ChangeUserRoleDTO { Role = "Admin" });
            var result = await _changeRoleValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ChangeRole_Validate_WhenRoleIsInvalid_ShouldFail()
        {
            var command = new ChangeUserRoleCommand("1", new ChangeUserRoleDTO { Role = "SuperUser" });
            var result = await _changeRoleValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Role must be one of: Admin, Doctor, Patient, Receptionist.");
        }

        [Fact]
        public async Task ChangeRole_Validate_WhenRoleIsEmpty_ShouldFail()
        {
            var command = new ChangeUserRoleCommand("1", new ChangeUserRoleDTO { Role = "" });
            var result = await _changeRoleValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Role is required.");
        }
    }
}
