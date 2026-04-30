using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Patients.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class PatientUseCaseHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;

        public PatientUseCaseHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetMedicalHistory_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

            var handler = new GetPatientMedicalHistoryHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetPatientMedicalHistoryQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetMedicalHistory_WhenCacheHit_ReturnsCachedResult()
        {
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var cached = new PatientMedicalHistoryDTO { PatientId = 1, PatientName = "John" };

            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _cacheMock.Setup(c => c.Get<PatientMedicalHistoryDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetPatientMedicalHistoryHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetPatientMedicalHistoryQuery(1), CancellationToken.None);

            result.Should().Be(cached);
        }

        [Fact]
        public async Task GetActiveAppointments_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(false);

            var handler = new GetPatientActiveAppointmentsHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetPatientActiveAppointmentsQuery(99, new PaginationRequest()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
