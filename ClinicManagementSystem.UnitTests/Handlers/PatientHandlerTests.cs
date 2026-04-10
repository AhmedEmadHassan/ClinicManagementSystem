using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Patients.Commands.Create;
using ClinicManagementSystem.Application.Features.Patients.Commands.Delete;
using ClinicManagementSystem.Application.Features.Patients.Commands.Update;
using ClinicManagementSystem.Application.Features.Patients.Queries.GetAll;
using ClinicManagementSystem.Application.Features.Patients.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class PatientHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public PatientHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetAll_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponsePatientDTO>
            {
                Data = new List<ResponsePatientDTO> { new() { Id = 1, Name = "John" } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponsePatientDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAllPatientsHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllPatientsQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Patients.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Patient, ResponsePatientDTO>>>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new ResponsePatientDTO { Id = 1, Name = "John" };
            _cacheMock.Setup(c => c.Get<ResponsePatientDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetPatientByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetPatientByIdQuery(1), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Patients.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenPatientExists_QueriesDatabaseAndSetsCache()
        {
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var dto = new ResponsePatientDTO { Id = 1, Name = "John", Gender = "Male" };

            _cacheMock.Setup(c => c.Get<ResponsePatientDTO>(It.IsAny<string>())).Returns((ResponsePatientDTO?)null);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _mapperMock.Setup(m => m.Map<ResponsePatientDTO>(patient)).Returns(dto);

            var handler = new GetPatientByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetPatientByIdQuery(1), CancellationToken.None);

            result.Should().Be(dto);
            _cacheMock.Verify(c => c.Set(It.IsAny<string>(), dto, null), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _cacheMock.Setup(c => c.Get<ResponsePatientDTO>(It.IsAny<string>())).Returns((ResponsePatientDTO?)null);
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

            var handler = new GetPatientByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetPatientByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_InvalidatesCacheAndReturnsDTO()
        {
            var dto = new CreatePatientDTO { Name = "John", Phone = "123", Gender = "Male" };
            var entity = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var response = new ResponsePatientDTO { Id = 1, Name = "John", Gender = "Male" };

            _mapperMock.Setup(m => m.Map<Patient>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.Patients.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ResponsePatientDTO>(entity)).Returns(response);

            var handler = new CreatePatientHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new CreatePatientCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("John");
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Patient), Times.Once);
        }

        [Fact]
        public async Task Update_WhenPatientNotFound_ThrowsNotFoundException()
        {
            var dto = new CreatePatientDTO { Name = "John", Phone = "123", Gender = "Male" };
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

            var handler = new UpdatePatientHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new UpdatePatientCommand(99, dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenPatientExists_InvalidatesCacheAndReturnsTrue()
        {
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeletePatientHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new DeletePatientCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Patient), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

            var handler = new DeletePatientHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new DeletePatientCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }
    }
}
