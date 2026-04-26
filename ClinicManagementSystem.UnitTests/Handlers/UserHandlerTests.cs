using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Users.Commands.Delete;
using ClinicManagementSystem.Application.Features.Users.Commands.Update;
using ClinicManagementSystem.Application.Features.Users.Queries.GetById;
using ClinicManagementSystem.Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class UserHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public UserHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }

        // --- GetById ---
        [Fact]
        public async Task GetById_WhenUserExists_ReturnsDTO()
        {
            var user = new ApplicationUser { Id = "1", UserName = "admin", Email = "admin@clinic.com" };
            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

            var handler = new GetUserByIdHandler(_userManagerMock.Object);
            var result = await handler.Handle(new GetUserByIdQuery("1"), CancellationToken.None);

            result.Should().NotBeNull();
            result.UserName.Should().Be("admin");
            result.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task GetById_WhenUserNotFound_ThrowsNotFoundException()
        {
            _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((ApplicationUser?)null);

            var handler = new GetUserByIdHandler(_userManagerMock.Object);
            var act = async () => await handler.Handle(new GetUserByIdQuery("99"), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // --- GetByRole ---
        [Fact]
        public async Task GetByRole_WhenRoleIsValid_ReturnsUsers()
        {
            var users = new List<ApplicationUser>
        {
            new() { Id = "1", UserName = "doctor1", Email = "doctor1@clinic.com" }
        };

            _userManagerMock.Setup(u => u.GetUsersInRoleAsync("Doctor")).ReturnsAsync(users);
            _userManagerMock.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "Doctor" });

            var handler = new GetUsersByRoleHandler(_userManagerMock.Object);
            var result = await handler.Handle(new GetUsersByRoleQuery("Doctor"), CancellationToken.None);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].Role.Should().Be("Doctor");
        }

        [Fact]
        public async Task GetByRole_WhenRoleIsInvalid_ThrowsBadRequestException()
        {
            var handler = new GetUsersByRoleHandler(_userManagerMock.Object);
            var act = async () => await handler.Handle(new GetUsersByRoleQuery("InvalidRole"), CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>();
        }

        // --- Update ---
        [Fact]
        public async Task Update_WhenUserExists_ReturnsUpdatedDTO()
        {
            var user = new ApplicationUser { Id = "1", UserName = "oldname", Email = "old@clinic.com" };
            var dto = new UpdateUserDTO { UserName = "newname", Email = "new@clinic.com" };

            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.FindByNameAsync("newname")).ReturnsAsync((ApplicationUser?)null);
            _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

            var handler = new UpdateUserHandler(_userManagerMock.Object);
            var result = await handler.Handle(new UpdateUserCommand("1", dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.UserName.Should().Be("newname");
            result.Email.Should().Be("new@clinic.com");
        }

        [Fact]
        public async Task Update_WhenUserNotFound_ThrowsNotFoundException()
        {
            var dto = new UpdateUserDTO { UserName = "newname", Email = "new@clinic.com" };
            _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((ApplicationUser?)null);

            var handler = new UpdateUserHandler(_userManagerMock.Object);
            var act = async () => await handler.Handle(new UpdateUserCommand("99", dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Update_WhenUsernameTaken_ThrowsDuplicateException()
        {
            var user = new ApplicationUser { Id = "1", UserName = "oldname", Email = "old@clinic.com" };
            var existingUser = new ApplicationUser { Id = "2", UserName = "takenname", Email = "taken@clinic.com" };
            var dto = new UpdateUserDTO { UserName = "takenname", Email = "new@clinic.com" };

            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.FindByNameAsync("takenname")).ReturnsAsync(existingUser);

            var handler = new UpdateUserHandler(_userManagerMock.Object);
            var act = async () => await handler.Handle(new UpdateUserCommand("1", dto), CancellationToken.None);

            await act.Should().ThrowAsync<DuplicateException>();
        }

        // --- Delete ---
        [Fact]
        public async Task Delete_WhenUserExists_ReturnsTrue()
        {
            var user = new ApplicationUser { Id = "1", UserName = "admin", Email = "admin@clinic.com" };
            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var handler = new DeleteUserHandler(_userManagerMock.Object);
            var result = await handler.Handle(new DeleteUserCommand("1"), CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WhenUserNotFound_ThrowsNotFoundException()
        {
            _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((ApplicationUser?)null);

            var handler = new DeleteUserHandler(_userManagerMock.Object);
            var act = async () => await handler.Handle(new DeleteUserCommand("99"), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // --- ChangeRole ---
        [Fact]
        public async Task ChangeRole_WhenValidRole_ReturnsUpdatedDTO()
        {
            var user = new ApplicationUser { Id = "1", UserName = "doctor1", Email = "doctor1@clinic.com" };
            var dto = new ChangeUserRoleDTO { Role = "Receptionist" };

            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Doctor" });
            _userManagerMock.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(user, "Receptionist")).ReturnsAsync(IdentityResult.Success);

            var handler = new ChangeUserRoleHandler(_userManagerMock.Object);
            var result = await handler.Handle(new ChangeUserRoleCommand("1", dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Role.Should().Be("Receptionist");
        }

        [Fact]
        public async Task ChangeRole_WhenUserNotFound_ThrowsNotFoundException()
        {
            var dto = new ChangeUserRoleDTO { Role = "Admin" };
            _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((ApplicationUser?)null);

            var handler = new ChangeUserRoleHandler(_userManagerMock.Object);
            var act = async () => await handler.Handle(new ChangeUserRoleCommand("99", dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ChangeRole_WhenInvalidRole_ThrowsBadRequestException()
        {
            var user = new ApplicationUser { Id = "1", UserName = "doctor1", Email = "doctor1@clinic.com" };
            var dto = new ChangeUserRoleDTO { Role = "InvalidRole" };

            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);

            var handler = new ChangeUserRoleHandler(_userManagerMock.Object);
            var act = async () => await handler.Handle(new ChangeUserRoleCommand("1", dto), CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>();
        }
    }
}
