using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Create;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Delete;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.UnitTests.Handlers
{
    public class DoctorHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        public DoctorHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetById_WhenDoctorExists_ReturnsDTO()
        {
            var specialization = new DoctorSpecialization { Id = 1, Name = "Cardiology" };
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            var dto = new ResponseDoctorDTO { Id = 1, Name = "Dr.Smith", Gender = "Male", DoctorSpecializationName = "Cardiology" };

            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _mapperMock.Setup(m => m.Map<ResponseDoctorDTO>(doctor)).Returns(dto);

            var handler = new GetDoctorByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new GetDoctorByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("Dr.Smith");
            result.DoctorSpecializationName.Should().Be("Cardiology");
        }

        [Fact]
        public async Task GetById_WhenDoctorNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(99)).ReturnsAsync((Doctor?)null);

            var handler = new GetDoctorByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new GetDoctorByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenSpecializationExists_ReturnsCreatedDTO()
        {
            var dto = new CreateDoctorDTO { Name = "Dr.Smith", Phone = "123", Gender = "Male", DoctorSpecializationId = 1 };
            var entity = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            var specialization = new DoctorSpecialization { Id = 1, Name = "Cardiology" };
            var response = new ResponseDoctorDTO { Id = 1, Name = "Dr.Smith", DoctorSpecializationName = "Cardiology" };

            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<Doctor>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.Doctors.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _mapperMock.Setup(m => m.Map<ResponseDoctorDTO>(entity)).Returns(response);

            var handler = new CreateDoctorHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = await handler.Handle(new CreateDoctorCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.DoctorSpecializationName.Should().Be("Cardiology");
        }

        [Fact]
        public async Task Create_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateDoctorDTO { Name = "Dr.Smith", Phone = "123", Gender = "Male", DoctorSpecializationId = 99 };
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(false);

            var handler = new CreateDoctorHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var act = async () => await handler.Handle(new CreateDoctorCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Delete_WhenDoctorExists_ReturnsTrue()
        {
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteDoctorHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new DeleteDoctorCommand(1), CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WhenDoctorNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(99)).ReturnsAsync((Doctor?)null);

            var handler = new DeleteDoctorHandler(_unitOfWorkMock.Object);
            var act = async () => await handler.Handle(new DeleteDoctorCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
