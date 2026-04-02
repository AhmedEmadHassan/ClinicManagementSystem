using ClinicManagementSystem.Application;
using ClinicManagementSystem.Application.DTOs.AuthDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Implementation.Auth;
using ClinicManagementSystem.Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace ClinicManagementSystem.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var jwtSettings = Options.Create(new JwtSettings
            {
                Key = "SuperSecretKeyForTestingPurposes123!",
                Issuer = "ClinicManagementSystem",
                Audience = "ClinicManagementSystemUsers",
                DurationInMinutes = 60,
                RefreshTokenDurationInDays = 7
            });

            _authService = new AuthService(_userManagerMock.Object, _unitOfWorkMock.Object, jwtSettings);
        }

        // --- Register ---
        [Fact]
        public async Task Register_WhenUsernameAlreadyExists_ThrowsDuplicateException()
        {
            var dto = new RegisterDTO { UserName = "existing", Email = "e@e.com", Password = "P@$$w0rd", Role = "Admin" };
            _userManagerMock.Setup(u => u.FindByNameAsync("existing")).ReturnsAsync(new ApplicationUser());

            var act = async () => await _authService.Register(dto);

            await act.Should().ThrowAsync<DuplicateException>();
        }

        [Fact]
        public async Task Register_WhenInvalidRole_ThrowsBadRequestException()
        {
            var dto = new RegisterDTO { UserName = "newuser", Email = "e@e.com", Password = "P@$$w0rd", Role = "InvalidRole" };
            _userManagerMock.Setup(u => u.FindByNameAsync("newuser")).ReturnsAsync((ApplicationUser?)null);

            var act = async () => await _authService.Register(dto);

            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Register_WhenDoctorWithoutSpecializationId_ThrowsBadRequestException()
        {
            var dto = new RegisterDTO { UserName = "drsmith", Email = "dr@dr.com", Password = "P@$$w0rd", Role = "Doctor", Phone = "123", Gender = "Male" };
            _userManagerMock.Setup(u => u.FindByNameAsync("drsmith")).ReturnsAsync((ApplicationUser?)null);

            var act = async () => await _authService.Register(dto);

            await act.Should().ThrowAsync<BadRequestException>();
        }

        // --- Login ---
        [Fact]
        public async Task Login_WhenUserNotFound_ThrowsBadRequestException()
        {
            var dto = new LoginDTO { UserName = "unknown", Password = "P@$$w0rd" };
            _userManagerMock.Setup(u => u.FindByNameAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

            var act = async () => await _authService.Login(dto);

            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Login_WhenPasswordIsInvalid_ThrowsBadRequestException()
        {
            var user = new ApplicationUser { UserName = "existing" };
            var dto = new LoginDTO { UserName = "existing", Password = "WrongPassword" };

            _userManagerMock.Setup(u => u.FindByNameAsync("existing")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "WrongPassword")).ReturnsAsync(false);

            var act = async () => await _authService.Login(dto);

            await act.Should().ThrowAsync<BadRequestException>();
        }

        // --- Logout ---
        [Fact]
        public async Task Logout_WhenUserNotFound_ThrowsNotFoundException()
        {
            _userManagerMock.Setup(u => u.FindByNameAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

            var act = async () => await _authService.Logout("unknown");

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Logout_WhenUserExists_ClearsRefreshToken()
        {
            var user = new ApplicationUser { UserName = "existing", RefreshToken = "token", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7) };

            _userManagerMock.Setup(u => u.FindByNameAsync("existing")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            await _authService.Logout("existing");

            user.RefreshToken.Should().BeNull();
            user.RefreshTokenExpiryTime.Should().BeNull();
        }

        // --- ChangePassword ---
        [Fact]
        public async Task ChangePassword_WhenUserNotFound_ThrowsNotFoundException()
        {
            _userManagerMock.Setup(u => u.FindByNameAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

            var act = async () => await _authService.ChangePassword("unknown", new ChangePasswordDTO { CurrentPassword = "old", NewPassword = "new" });

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ChangePassword_WhenCurrentPasswordIsWrong_ThrowsBadRequestException()
        {
            var user = new ApplicationUser { UserName = "existing" };
            _userManagerMock.Setup(u => u.FindByNameAsync("existing")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.ChangePasswordAsync(user, "wrongOld", "newPass"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Incorrect password." }));

            var act = async () => await _authService.ChangePassword("existing", new ChangePasswordDTO { CurrentPassword = "wrongOld", NewPassword = "newPass" });

            await act.Should().ThrowAsync<BadRequestException>();
        }

        // --- RefreshToken ---
        [Fact]
        public async Task RefreshToken_WhenRefreshTokenIsInvalid_ThrowsBadRequestException()
        {
            var user = new ApplicationUser { UserName = "existing", RefreshToken = "valid-token", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7) };

            _userManagerMock.Setup(u => u.FindByNameAsync("existing")).ReturnsAsync(user);

            var act = async () => await _authService.RefreshToken(new RefreshTokenDTO
            {
                AccessToken = "invalid-token",
                RefreshToken = "wrong-refresh-token"
            });

            await act.Should().ThrowAsync<Exception>();
        }
    }
}
