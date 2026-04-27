using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class DoctorUseCaseHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;

        public DoctorUseCaseHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetBySpecialization_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(false);

            var handler = new GetDoctorsBySpecializationHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetDoctorsBySpecializationQuery(99, new PaginationRequest()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetBySpecialization_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseDoctorDTO> { Data = new(), PageNumber = 1, PageSize = 10, TotalCount = 0 };

            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(true);
            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseDoctorDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetDoctorsBySpecializationHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetDoctorsBySpecializationQuery(1, new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
        }

        [Fact]
        public async Task GetAvailability_WhenDoctorNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(99)).ReturnsAsync((Doctor?)null);

            var handler = new GetDoctorAvailabilityHandler(_unitOfWorkMock.Object);
            var act = async () => await handler.Handle(
                new GetDoctorAvailabilityQuery(99, DateOnly.FromDateTime(DateTime.UtcNow)),
                CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
