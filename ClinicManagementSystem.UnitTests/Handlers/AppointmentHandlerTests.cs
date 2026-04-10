using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Create;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Delete;
using ClinicManagementSystem.Application.Features.Appointments.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Appointments.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class AppointmentHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public AppointmentHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetAll_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseAppointmentDTO>
            {
                Data = new List<ResponseAppointmentDTO> { new() { Id = 1 } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseAppointmentDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAllAppointmentsHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllAppointmentsQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Appointments.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Appointment, ResponseAppointmentDTO>>>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new ResponseAppointmentDTO { Id = 1 };
            _cacheMock.Setup(c => c.Get<ResponseAppointmentDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAppointmentByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAppointmentByIdQuery(1), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Appointments.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenAppointmentNotFound_ThrowsNotFoundException()
        {
            _cacheMock.Setup(c => c.Get<ResponseAppointmentDTO>(It.IsAny<string>())).Returns((ResponseAppointmentDTO?)null);
            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(99)).ReturnsAsync((Appointment?)null);

            var handler = new GetAppointmentByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetAppointmentByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_InvalidatesCacheAndReturnsDTO()
        {
            var dto = new CreateAppointmentDTO { PatientId = 1, DoctorId = 1, AppointmentStateId = 1, AppointmentDate = DateOnly.FromDateTime(DateTime.Now) };
            var entity = new Appointment { Id = 1, PatientId = 1, DoctorId = 1, AppointmentStateId = 1 };
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            var state = new AppointmentState { Id = 1, Name = "Scheduled" };
            var response = new ResponseAppointmentDTO { Id = 1, PatientName = "John", DoctorName = "Dr.Smith" };

            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.AppointmentStates.AnyAsync(It.IsAny<Expression<Func<AppointmentState, bool>>>())).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<Appointment>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.Appointments.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(1)).ReturnsAsync(state);
            _mapperMock.Setup(m => m.Map<ResponseAppointmentDTO>(entity)).Returns(response);

            var handler = new CreateAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new CreateAppointmentCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.PatientName.Should().Be("John");
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Appointment), Times.Once);
        }

        [Fact]
        public async Task Create_WhenPatientNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateAppointmentDTO { PatientId = 99, DoctorId = 1, AppointmentStateId = 1, AppointmentDate = DateOnly.FromDateTime(DateTime.Now) };
            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(false);

            var handler = new CreateAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new CreateAppointmentCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenAppointmentExists_InvalidatesCacheAndReturnsTrue()
        {
            var appointment = new Appointment { Id = 1, PatientId = 1, DoctorId = 1, AppointmentStateId = 1 };
            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(1)).ReturnsAsync(appointment);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteAppointmentHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new DeleteAppointmentCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Appointment), Times.Once);
        }
    }
}
