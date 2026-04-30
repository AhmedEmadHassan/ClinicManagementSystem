using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Appointments.Commands.Update;
using ClinicManagementSystem.Application.Features.Appointments.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class AppointmentUseCaseHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public AppointmentUseCaseHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetByDoctor_WhenDoctorNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(false);

            var handler = new GetAppointmentsByDoctorHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetAppointmentsByDoctorQuery(99, new PaginationRequest()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetByDoctor_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseAppointmentDTO> { Data = new(), PageNumber = 1, PageSize = 10, TotalCount = 0 };

            _unitOfWorkMock.Setup(u => u.Doctors.AnyAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);
            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseAppointmentDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAppointmentsByDoctorHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAppointmentsByDoctorQuery(1, new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Appointments.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Appointment, ResponseAppointmentDTO>>>(), It.IsAny<Expression<Func<Appointment, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task GetByPatient_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(false);

            var handler = new GetAppointmentsByPatientHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetAppointmentsByPatientQuery(99, new PaginationRequest()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Cancel_WhenAppointmentNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(99)).ReturnsAsync((Appointment?)null);

            var handler = new CancelAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new CancelAppointmentCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Cancel_WhenAlreadyCancelled_ThrowsBadRequestException()
        {
            var cancelledState = new AppointmentState { Id = 2, Name = "Cancelled" };
            var appointment = new Appointment { Id = 1, AppointmentStateId = 2 };

            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(1)).ReturnsAsync(appointment);
            _unitOfWorkMock
                .Setup(u => u.AppointmentStates.FirstOrDefaultAsync(It.IsAny<Expression<Func<AppointmentState, bool>>>()))
                .ReturnsAsync(cancelledState);

            var handler = new CancelAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new CancelAppointmentCommand(1), CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Appointment is already cancelled.");
        }

        [Fact]
        public async Task Reschedule_WhenAppointmentNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Appointments.GetByIdAsync(99)).ReturnsAsync((Appointment?)null);

            var handler = new RescheduleAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(
                new RescheduleAppointmentCommand(99, new RescheduleAppointmentDTO { AppointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) }),
                CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
