using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Create;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Update;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class AppointmentValidatorTests
    {
        private readonly CreateAppointmentValidator _createValidator = new();
        private readonly UpdateAppointmentValidator _updateValidator = new();

        [Fact]
        public async Task Create_Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new CreateAppointmentCommand(new CreateAppointmentDTO
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentStateId = 1,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now)
            });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Create_Validate_WhenPatientIdIsZero_ShouldFail()
        {
            var command = new CreateAppointmentCommand(new CreateAppointmentDTO
            {
                PatientId = 0,
                DoctorId = 1,
                AppointmentStateId = 1,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now)
            });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "PatientId must be a valid id.");
        }

        [Fact]
        public async Task Create_Validate_WhenAppointmentDateIsEmpty_ShouldFail()
        {
            var command = new CreateAppointmentCommand(new CreateAppointmentDTO
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentStateId = 1,
                AppointmentDate = default
            });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "AppointmentDate is required.");
        }

        [Fact]
        public async Task Update_Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new UpdateAppointmentCommand(1, new UpdateAppointmentDTO
            {
                AppointmentStateId = 1,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now)
            });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Update_Validate_WhenAppointmentStateIdIsZero_ShouldFail()
        {
            var command = new UpdateAppointmentCommand(1, new UpdateAppointmentDTO
            {
                AppointmentStateId = 0,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now)
            });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "AppointmentStateId must be a valid id.");
        }

        [Fact]
        public async Task Update_Validate_WhenAppointmentDateIsEmpty_ShouldFail()
        {
            var command = new UpdateAppointmentCommand(1, new UpdateAppointmentDTO
            {
                AppointmentStateId = 1,
                AppointmentDate = default
            });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "AppointmentDate is required.");
        }
    }
}
