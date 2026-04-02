using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Patients.Commands.Create;
using ClinicManagementSystem.Application.Features.Patients.Commands.Delete;
using ClinicManagementSystem.Application.Features.Patients.Commands.Update;
using ClinicManagementSystem.Application.Features.Patients.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class PatientHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        public PatientHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetById_WhenPatientExists_ReturnsDTO()
        {
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var dto = new ResponsePatientDTO { Id = 1, Name = "John", Gender = "Male" };

            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _mapperMock.Setup(m => m.Map<ResponsePatientDTO>(patient)).Returns(dto);

            var handler = new GetPatientByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new GetPatientByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("John");
            result.Gender.Should().Be("Male");
        }

        [Fact]
        public async Task GetById_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

            var handler = new GetPatientByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new GetPatientByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenValidInput_ReturnsCreatedDTO()
        {
            var dto = new CreatePatientDTO { Name = "John", Phone = "123", Gender = "Male" };
            var entity = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            var response = new ResponsePatientDTO { Id = 1, Name = "John", Gender = "Male" };

            _mapperMock.Setup(m => m.Map<Patient>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.Patients.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ResponsePatientDTO>(entity)).Returns(response);

            var handler = new CreatePatientHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new CreatePatientCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("John");
        }

        [Fact]
        public async Task Update_WhenPatientNotFound_ThrowsNotFoundException()
        {
            var dto = new CreatePatientDTO { Name = "John", Phone = "123", Gender = "Male" };
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

            var handler = new UpdatePatientHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new UpdatePatientCommand(99, dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Delete_WhenPatientExists_ReturnsTrue()
        {
            var patient = new Patient { Id = 1, Name = "John", Phone = "123", Gender = true };
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(patient);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeletePatientHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new DeletePatientCommand(1), CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WhenPatientNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Patients.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

            var handler = new DeletePatientHandler(_unitOfWorkMock.Object);
            var act = async () => await handler.Handle(new DeletePatientCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
