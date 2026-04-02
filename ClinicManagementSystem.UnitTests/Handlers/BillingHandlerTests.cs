using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Billings.Commands.Create;
using ClinicManagementSystem.Application.Features.Billings.Commands.Delete;
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

        public BillingHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetById_WhenBillingExists_ReturnsDTO()
        {
            var billing = new Billing { Id = 1, PatientId = 1, SessionId = 1, Amount = 100, Description = "Consultation" };
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var dto = new ResponseBillingDTO { Id = 1, PatientName = "John", Amount = 100 };

            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(1)).ReturnsAsync(billing);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _mapperMock.Setup(m => m.Map<ResponseBillingDTO>(billing)).Returns(dto);

            var handler = new GetBillingByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new GetBillingByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.PatientName.Should().Be("John");
            result.Amount.Should().Be(100);
        }

        [Fact]
        public async Task GetById_WhenBillingNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(99)).ReturnsAsync((Billing?)null);

            var handler = new GetBillingByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new GetBillingByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_ReturnsCreatedDTO()
        {
            var dto = new CreateBillingDTO { SessionId = 1, PatientId = 1, Amount = 100, Description = "Consultation" };
            var entity = new Billing { Id = 1, SessionId = 1, PatientId = 1, Amount = 100, Description = "Consultation" };
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var response = new ResponseBillingDTO { Id = 1, PatientName = "John", Amount = 100 };

            _unitOfWorkMock.Setup(u => u.Sessions.AnyAsync(It.IsAny<Expression<Func<Session, bool>>>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Patients.AnyAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<Billing>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.Billings.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _mapperMock.Setup(m => m.Map<ResponseBillingDTO>(entity)).Returns(response);

            var handler = new CreateBillingHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new CreateBillingCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Amount.Should().Be(100);
        }

        [Fact]
        public async Task Delete_WhenBillingExists_ReturnsTrue()
        {
            var billing = new Billing { Id = 1, PatientId = 1, SessionId = 1, Amount = 100, Description = "Consultation" };
            _unitOfWorkMock.Setup(u => u.Billings.GetByIdAsync(1)).ReturnsAsync(billing);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteBillingHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new DeleteBillingCommand(1), CancellationToken.None);

            result.Should().BeTrue();
        }
    }
}
