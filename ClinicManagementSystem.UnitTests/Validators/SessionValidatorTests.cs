using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Create;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Update;
using FluentAssertions;

namespace ClinicManagementSystem.UnitTests.Validators
{
    public class SessionValidatorTests
    {
        private readonly CreateSessionValidator _createValidator = new();
        private readonly UpdateSessionValidator _updateValidator = new();

        [Fact]
        public async Task Create_Validate_WhenAppointmentIdIsValid_ShouldPass()
        {
            var command = new CreateSessionCommand(new CreateSessionDTO { AppointmentId = 1 });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Create_Validate_WhenAppointmentIdIsZero_ShouldFail()
        {
            var command = new CreateSessionCommand(new CreateSessionDTO { AppointmentId = 0 });
            var result = await _createValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "AppointmentId must be a valid id.");
        }

        [Fact]
        public async Task Update_Validate_WhenConsultationNotesExceedsMaxLength_ShouldFail()
        {
            var command = new UpdateSessionCommand(1, new UpdateSessionDTO { ConsultationNotes = new string('A', 1001) });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "ConsultationNotes must not exceed 1000 characters.");
        }

        [Fact]
        public async Task Update_Validate_WhenPrescriptionsExceedsMaxLength_ShouldFail()
        {
            var command = new UpdateSessionCommand(1, new UpdateSessionDTO { Prescriptions = new string('A', 1001) });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Prescriptions must not exceed 1000 characters.");
        }

        [Fact]
        public async Task Update_Validate_WhenAllFieldsAreNull_ShouldPass()
        {
            var command = new UpdateSessionCommand(1, new UpdateSessionDTO { ConsultationNotes = null, Prescriptions = null });
            var result = await _updateValidator.ValidateAsync(command);
            result.IsValid.Should().BeTrue();
        }
    }
}
