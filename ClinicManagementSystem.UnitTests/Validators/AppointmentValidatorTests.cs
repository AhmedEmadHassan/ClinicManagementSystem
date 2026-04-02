using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Create;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class AppointmentValidatorTests
    {
        private readonly CreateAppointmentValidator _validator = new();

        [Fact]
        public async Task Validate_WhenAllFieldsValid_ShouldPass()
        {
            var command = new CreateAppointmentCommand(new CreateAppointmentDTO
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentStateId = 1,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now)
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WhenPatientIdIsZero_ShouldFail()
        {
            var command = new CreateAppointmentCommand(new CreateAppointmentDTO
            {
                PatientId = 0,
                DoctorId = 1,
                AppointmentStateId = 1,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now)
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "PatientId must be a valid id.");
        }

        [Fact]
        public async Task Validate_WhenAppointmentDateIsEmpty_ShouldFail()
        {
            var command = new CreateAppointmentCommand(new CreateAppointmentDTO
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentStateId = 1,
                AppointmentDate = default
            });
            var result = await _validator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "AppointmentDate is required.");
        }
    }
}
