using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Billings.Commands.Update;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class BillingUseCaseHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public BillingUseCaseHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetUnpaidByPatient_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(false);

            var handler = new GetUnpaidBillingsByPatientHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetUnpaidBillingsByPatientQuery(99, new PaginationRequest()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetUnpaidByPatient_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseBillingDTO> { Data = new(), PageNumber = 1, PageSize = 10, TotalCount = 0 };

            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(true);
            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseBillingDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetUnpaidBillingsByPatientHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetUnpaidBillingsByPatientQuery(1, new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
        }

        [Fact]
        public async Task MarkAsPaid_WhenBillingNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(99)).ReturnsAsync((Billing?)null);

            var handler = new MarkBillingAsPaidHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new MarkBillingAsPaidCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task MarkAsPaid_WhenAlreadyPaid_ThrowsBadRequestException()
        {
            var billing = new Billing { Id = 1, IsPaid = true, PatientId = 1, SessionId = 1, Amount = 100, Description = "Test" };
            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(1)).ReturnsAsync(billing);

            var handler = new MarkBillingAsPaidHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new MarkBillingAsPaidCommand(1), CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task GetRevenue_WhenStartDateAfterEndDate_ThrowsBadRequestException()
        {
            var handler = new GetRevenueForPeriodHandler(_unitOfWorkMock.Object);
            var act = async () => await handler.Handle(
                new GetRevenueForPeriodQuery(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))),
                CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>();
        }
    }
}
