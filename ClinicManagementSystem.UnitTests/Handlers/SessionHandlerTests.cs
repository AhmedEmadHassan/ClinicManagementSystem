using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Create;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Delete;
using ClinicManagementSystem.Application.Features.Sessions.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class SessionHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        public SessionHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetById_WhenSessionExists_ReturnsDTO()
        {
            var session = new Session { Id = 1, PatientId = 1, DoctorId = 1, AppointmentId = 1 };
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            var dto = new ResponseSessionDTO { Id = 1, PatientName = "John", DoctorName = "Dr.Smith" };

            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(1)).ReturnsAsync(session);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _mapperMock.Setup(m => m.Map<ResponseSessionDTO>(session)).Returns(dto);

            var handler = new GetSessionByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new GetSessionByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.PatientName.Should().Be("John");
            result.DoctorName.Should().Be("Dr.Smith");
        }

        [Fact]
        public async Task GetById_WhenSessionNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(99)).ReturnsAsync((Session?)null);

            var handler = new GetSessionByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new GetSessionByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_ReturnsCreatedDTO()
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

            var handler = new CreateSessionHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new CreateSessionCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.PatientName.Should().Be("John");
        }

        [Fact]
        public async Task Delete_WhenSessionExists_ReturnsTrue()
        {
            var session = new Session { Id = 1, PatientId = 1, DoctorId = 1, AppointmentId = 1 };
            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(1)).ReturnsAsync(session);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteSessionHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new DeleteSessionCommand(1), CancellationToken.None);

            result.Should().BeTrue();
        }
    }
}
