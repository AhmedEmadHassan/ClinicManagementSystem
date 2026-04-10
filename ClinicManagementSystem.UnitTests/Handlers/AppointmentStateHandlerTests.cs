using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Create;
using ClinicManagementSystem.Application.Features.AppointmentStates.Commands.Delete;
using ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetAll;
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
        private readonly Mock<ICacheService> _cacheMock;

        public AppointmentStateHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetAll_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseAppointmentStateDTO>
            {
                Data = new List<ResponseAppointmentStateDTO> { new() { Id = 1, Name = "Scheduled" } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseAppointmentStateDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAllAppointmentStatesHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllAppointmentStatesQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.AppointmentStates.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<AppointmentState, ResponseAppointmentStateDTO>>>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new ResponseAppointmentStateDTO { Id = 1, Name = "Scheduled" };
            _cacheMock.Setup(c => c.Get<ResponseAppointmentStateDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAppointmentStateByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAppointmentStateByIdQuery(1), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.AppointmentStates.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenStateNotFound_ThrowsNotFoundException()
        {
            _cacheMock.Setup(c => c.Get<ResponseAppointmentStateDTO>(It.IsAny<string>())).Returns((ResponseAppointmentStateDTO?)null);
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(99)).ReturnsAsync((AppointmentState?)null);

            var handler = new GetAppointmentStateByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetAppointmentStateByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenNameIsUnique_InvalidatesCacheAndReturnsDTO()
        {
            var dto = new CreateAppointmentStateDTO { Name = "Cancelled" };
            var entity = new AppointmentState { Id = 1, Name = "Cancelled" };
            var response = new ResponseAppointmentStateDTO { Id = 1, Name = "Cancelled" };

            _unitOfWorkMock.Setup(u => u.AppointmentStates.AnyAsync(It.IsAny<Expression<Func<AppointmentState, bool>>>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<AppointmentState>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.AppointmentStates.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ResponseAppointmentStateDTO>(entity)).Returns(response);

            var handler = new CreateAppointmentStateHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new CreateAppointmentStateCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("Cancelled");
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.AppointmentState), Times.Once);
        }

        [Fact]
        public async Task Create_WhenNameIsDuplicate_ThrowsDuplicateException()
        {
            var dto = new CreateAppointmentStateDTO { Name = "Scheduled" };
            _unitOfWorkMock.Setup(u => u.AppointmentStates.AnyAsync(It.IsAny<Expression<Func<AppointmentState, bool>>>())).ReturnsAsync(true);

            var handler = new CreateAppointmentStateHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new CreateAppointmentStateCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<DuplicateException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenStateExists_InvalidatesCacheAndReturnsTrue()
        {
            var state = new AppointmentState { Id = 1, Name = "Scheduled" };
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(1)).ReturnsAsync(state);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteAppointmentStateHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new DeleteAppointmentStateCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.AppointmentState), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenStateNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(99)).ReturnsAsync((AppointmentState?)null);

            var handler = new DeleteAppointmentStateHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new DeleteAppointmentStateCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }
    }
}
