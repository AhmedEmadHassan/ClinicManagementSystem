using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Billings.Commands.Create;
using ClinicManagementSystem.Application.Features.Billings.Commands.Delete;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Billings.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class BillingHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public BillingHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetAll_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseBillingDTO>
            {
                Data = new List<ResponseBillingDTO> { new() { Id = 1 } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseBillingDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAllBillingsHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllBillingsQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Billings.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Billing, ResponseBillingDTO>>>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new ResponseBillingDTO { Id = 1 };
            _cacheMock.Setup(c => c.Get<ResponseBillingDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetBillingByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetBillingByIdQuery(1), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Billings.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenBillingNotFound_ThrowsNotFoundException()
        {
            _cacheMock.Setup(c => c.Get<ResponseBillingDTO>(It.IsAny<string>())).Returns((ResponseBillingDTO?)null);
            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(99)).ReturnsAsync((Billing?)null);

            var handler = new GetBillingByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetBillingByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_InvalidatesCacheAndReturnsDTO()
        {
            var dto = new CreateBillingDTO { SessionId = 1, Amount = 100, Description = "Consultation" };
            var session = new Session { Id = 1, PatientId = 1, DoctorId = 1, AppointmentId = 1 };
            var entity = new Billing { Id = 1, SessionId = 1, PatientId = 1, Amount = 100, Description = "Consultation" };
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var response = new ResponseBillingDTO { Id = 1, PatientName = "John", Amount = 100 };

            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(1)).ReturnsAsync(session);
            _mapperMock.Setup(m => m.Map<Billing>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.Billings.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _mapperMock.Setup(m => m.Map<ResponseBillingDTO>(entity)).Returns(response);

            var handler = new CreateBillingHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new CreateBillingCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Amount.Should().Be(100);
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Billing), Times.Once);
        }

        [Fact]
        public async Task Create_WhenSessionNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateBillingDTO { SessionId = 99, Amount = 100, Description = "Consultation" };
            _unitOfWorkMock.Setup(u => u.Sessions.GetByIdAsync(99)).ReturnsAsync((Session?)null);

            var handler = new CreateBillingHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new CreateBillingCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBillingExists_InvalidatesCacheAndReturnsTrue()
        {
            var billing = new Billing { Id = 1, PatientId = 1, SessionId = 1, Amount = 100, Description = "Consultation" };
            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(1)).ReturnsAsync(billing);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteBillingHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new DeleteBillingCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Billing), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBillingNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(99)).ReturnsAsync((Billing?)null);

            var handler = new DeleteBillingHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new DeleteBillingCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }
    }
}
