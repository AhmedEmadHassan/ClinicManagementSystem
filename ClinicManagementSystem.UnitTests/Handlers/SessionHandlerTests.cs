using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Create;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Delete;
using ClinicManagementSystem.Application.Features.Sessions.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Sessions.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class SessionHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public SessionHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetAll_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseSessionDTO>
            {
                Data = new List<ResponseSessionDTO> { new() { Id = 1 } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseSessionDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAllSessionsHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllSessionsQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Sessions.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Session, ResponseSessionDTO>>>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new ResponseSessionDTO { Id = 1 };
            _cacheMock.Setup(c => c.Get<ResponseSessionDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetSessionByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetSessionByIdQuery(1), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Sessions.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenSessionNotFound_ThrowsNotFoundException()
        {
            _cacheMock.Setup(c => c.Get<ResponseSessionDTO>(It.IsAny<string>())).Returns((ResponseSessionDTO?)null);
            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(99)).ReturnsAsync((Session?)null);

            var handler = new GetSessionByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetSessionByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_InvalidatesCacheAndReturnsDTO()
        {
            var dto = new CreateSessionDTO { AppointmentId = 1 };
            var appointment = new Appointment { Id = 1, PatientId = 1, DoctorId = 1, AppointmentStateId = 1 };
            var entity = new Session { Id = 1, AppointmentId = 1, PatientId = 1, DoctorId = 1 };
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            var response = new ResponseSessionDTO { Id = 1, PatientName = "John", DoctorName = "Dr.Smith" };

            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(1)).ReturnsAsync(appointment);
            _mapperMock.Setup(m => m.Map<Session>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.Sessions.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _mapperMock.Setup(m => m.Map<ResponseSessionDTO>(entity)).Returns(response);

            var handler = new CreateSessionHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new CreateSessionCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.PatientName.Should().Be("John");
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Session), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSessionExists_InvalidatesCacheAndReturnsTrue()
        {
            var session = new Session { Id = 1, PatientId = 1, DoctorId = 1, AppointmentId = 1 };
            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(1)).ReturnsAsync(session);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteSessionHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new DeleteSessionCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Session), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSessionNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(99)).ReturnsAsync((Session?)null);

            var handler = new DeleteSessionHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new DeleteSessionCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }
    }
}
