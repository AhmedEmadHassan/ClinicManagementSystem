using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Create;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Delete;
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

        public AppointmentHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetById_WhenAppointmentExists_ReturnsDTO()
        {
            var appointment = new Appointment { Id = 1, PatientId = 1, DoctorId = 1, AppointmentStateId = 1, AppointmentDate = DateOnly.FromDateTime(DateTime.Now) };
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            var state = new AppointmentState { Id = 1, Name = "Scheduled" };
            var dto = new ResponseAppointmentDTO { Id = 1, PatientName = "John", DoctorName = "Dr.Smith", AppointmentStateName = "Scheduled" };

            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(1)).ReturnsAsync(appointment);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _unitOfWorkMock.Setup(u => u.AppointmentStates.GetByIdAsync(1)).ReturnsAsync(state);
            _mapperMock.Setup(m => m.Map<ResponseAppointmentDTO>(appointment)).Returns(dto);

            var handler = new GetAppointmentByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new GetAppointmentByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.PatientName.Should().Be("John");
            result.DoctorName.Should().Be("Dr.Smith");
        }

        [Fact]
        public async Task GetById_WhenAppointmentNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(99)).ReturnsAsync((Appointment?)null);

            var handler = new GetAppointmentByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new GetAppointmentByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_ReturnsCreatedDTO()
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

            var handler = new CreateAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new CreateAppointmentCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.PatientName.Should().Be("John");
        }

        [Fact]
        public async Task Create_WhenPatientNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateAppointmentDTO { PatientId = 99, DoctorId = 1, AppointmentStateId = 1, AppointmentDate = DateOnly.FromDateTime(DateTime.Now) };
            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(false);

            var handler = new CreateAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new CreateAppointmentCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Delete_WhenAppointmentExists_ReturnsTrue()
        {
            var appointment = new Appointment { Id = 1, PatientId = 1, DoctorId = 1, AppointmentStateId = 1 };
            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(1)).ReturnsAsync(appointment);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteAppointmentHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new DeleteAppointmentCommand(1), CancellationToken.None);

            result.Should().BeTrue();
        }
    }
}
