using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Create;
using ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Delete;
using ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class AppointmentStateHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        public AppointmentStateHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetById_WhenStateExists_ReturnsDTO()
        {
            var state = new AppointmentState { Id = 1, Name = "Scheduled" };
            var dto = new ResponseAppointmentStateDTO { Id = 1, Name = "Scheduled" };

            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(1)).ReturnsAsync(state);
            _mapperMock.Setup(m => m.Map<ResponseAppointmentStateDTO>(state)).Returns(dto);

            var handler = new GetAppointmentStateByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new GetAppointmentStateByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("Scheduled");
        }

        [Fact]
        public async Task GetById_WhenStateNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(99)).ReturnsAsync((AppointmentState?)null);

            var handler = new GetAppointmentStateByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new GetAppointmentStateByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenNameIsUnique_ReturnsCreatedDTO()
        {
            var dto = new CreateAppointmentStateDTO { Name = "Cancelled" };
            var entity = new AppointmentState { Id = 1, Name = "Cancelled" };
            var response = new ResponseAppointmentStateDTO { Id = 1, Name = "Cancelled" };

            _unitOfWorkMock.Setup(u => u.AppointmentStates.AnyAsync(It.IsAny<Expression<Func<AppointmentState, bool>>>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<AppointmentState>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.AppointmentStates.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ResponseAppointmentStateDTO>(entity)).Returns(response);

            var handler = new CreateAppointmentStateHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new CreateAppointmentStateCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("Cancelled");
        }

        [Fact]
        public async Task Create_WhenNameIsDuplicate_ThrowsDuplicateException()
        {
            var dto = new CreateAppointmentStateDTO { Name = "Scheduled" };
            _unitOfWorkMock.Setup(u => u.AppointmentStates.AnyAsync(It.IsAny<Expression<Func<AppointmentState, bool>>>())).ReturnsAsync(true);

            var handler = new CreateAppointmentStateHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new CreateAppointmentStateCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<DuplicateException>();
        }

        [Fact]
        public async Task Delete_WhenStateExists_ReturnsTrue()
        {
            var state = new AppointmentState { Id = 1, Name = "Scheduled" };
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(1)).ReturnsAsync(state);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteAppointmentStateHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new DeleteAppointmentStateCommand(1), CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WhenStateNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(99)).ReturnsAsync((AppointmentState?)null);

            var handler = new DeleteAppointmentStateHandler(_unitOfWorkMock.Object);
            var act = async () => await handler.Handle(new DeleteAppointmentStateCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
