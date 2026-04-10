using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Create;
using ClinicManagementSystem.Application.Features.Doctors.Commands.Delete;
using ClinicManagementSystem.Application.Features.Doctors.Queries.GetAll;
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
        private readonly Mock<ICacheService> _cacheMock;

        public DoctorHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        [Fact]
        public async Task GetAll_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseDoctorDTO>
            {
                Data = new List<ResponseDoctorDTO> { new() { Id = 1, Name = "Dr.Smith" } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseDoctorDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAllDoctorsHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllDoctorsQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Doctors.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Doctor, ResponseDoctorDTO>>>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new ResponseDoctorDTO { Id = 1, Name = "Dr.Smith" };
            _cacheMock.Setup(c => c.Get<ResponseDoctorDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetDoctorByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetDoctorByIdQuery(1), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.Doctors.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenDoctorExists_QueriesDatabaseAndSetsCache()
        {
            var specialization = new DoctorSpecialization { Id = 1, Name = "Cardiology" };
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            var dto = new ResponseDoctorDTO { Id = 1, Name = "Dr.Smith", DoctorSpecializationName = "Cardiology" };

            _cacheMock.Setup(c => c.Get<ResponseDoctorDTO>(It.IsAny<string>())).Returns((ResponseDoctorDTO?)null);
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _mapperMock.Setup(m => m.Map<ResponseDoctorDTO>(doctor)).Returns(dto);

            var handler = new GetDoctorByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetDoctorByIdQuery(1), CancellationToken.None);

            result.Should().Be(dto);
            _cacheMock.Verify(c => c.Set(It.IsAny<string>(), dto, null), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenDoctorNotFound_ThrowsNotFoundException()
        {
            _cacheMock.Setup(c => c.Get<ResponseDoctorDTO>(It.IsAny<string>())).Returns((ResponseDoctorDTO?)null);
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(99)).ReturnsAsync((Doctor?)null);

            var handler = new GetDoctorByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetDoctorByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_WhenSpecializationExists_InvalidatesCacheAndReturnsDTO()
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

            var handler = new CreateDoctorHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new CreateDoctorCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.DoctorSpecializationName.Should().Be("Cardiology");
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Doctor), Times.Once);
        }

        [Fact]
        public async Task Create_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateDoctorDTO { Name = "Dr.Smith", Phone = "123", Gender = "Male", DoctorSpecializationId = 99 };
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(false);

            var handler = new CreateDoctorHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new CreateDoctorCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenDoctorExists_InvalidatesCacheAndReturnsTrue()
        {
            var doctor = new Doctor { Id = 1, Name = "Dr.Smith", Phone = "123", Gender = true, DoctorSpecializationId = 1 };
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(1)).ReturnsAsync(doctor);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteDoctorHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new DeleteDoctorCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.Doctor), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenDoctorNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Doctors.GetByIdAsync(99)).ReturnsAsync((Doctor?)null);

            var handler = new DeleteDoctorHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new DeleteDoctorCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }
    }
}
