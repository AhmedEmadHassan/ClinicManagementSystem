using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Sessions.Commands.Update;
using ClinicManagementSystem.Application.Features.Sessions.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class SessionUseCaseHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public SessionUseCaseHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetByPatient_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(false);

            var handler = new GetSessionsByPatientHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetSessionsByPatientQuery(99, new PaginationRequest()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetByDoctor_WhenDoctorNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(false);

            var handler = new GetSessionsByDoctorHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetSessionsByDoctorQuery(99, new PaginationRequest()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task StartFromAppointment_WhenAppointmentNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(99)).ReturnsAsync((Appointment?)null);

            var handler = new StartSessionFromAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new StartSessionFromAppointmentCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task StartFromAppointment_WhenSessionAlreadyExists_ThrowsDuplicateException()
        {
            var scheduledState = new AppointmentState { Id = 1, Name = "Scheduled" };
            var appointment = new Appointment { Id = 1, AppointmentStateId = 1, PatientId = 1, DoctorId = 1 };

            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(1)).ReturnsAsync(appointment);
            _unitOfWorkMock
                    .SetupSequence(u => u.AppointmentStates.FirstOrDefaultAsync(It.IsAny<Expression<Func<AppointmentState, bool>>>()))
                    .ReturnsAsync(scheduledState)
                    .ReturnsAsync((AppointmentState?)null);
            _unitOfWorkMock.Setup(u => u.Sessions.AnyAsync(It.IsAny<Expression<Func<Session, bool>>>())).ReturnsAsync(true);

            var handler = new StartSessionFromAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new StartSessionFromAppointmentCommand(1), CancellationToken.None);

            await act.Should().ThrowAsync<DuplicateException>()
                    .WithMessage("A session already exists for this appointment.");
        }
    }
}
