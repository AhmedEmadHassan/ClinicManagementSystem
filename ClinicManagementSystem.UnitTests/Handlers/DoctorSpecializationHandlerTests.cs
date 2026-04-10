using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Create;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Delete;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Update;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetAll;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetById;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ClinicManagementSystem.Tests.Handlers
{
    public class DoctorSpecializationHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        public DoctorSpecializationHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
        }

        // --- GetAll ---
        [Fact]
        public async Task GetAll_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new PaginatedResponse<ResponseDoctorSpecializationDTO>
            {
                Data = new List<ResponseDoctorSpecializationDTO> { new() { Id = 1, Name = "Cardiology" } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseDoctorSpecializationDTO>>(It.IsAny<string>())).Returns(cached);

            var handler = new GetAllDoctorSpecializationsHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllDoctorSpecializationsQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.DoctorSpecializations.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<DoctorSpecialization, ResponseDoctorSpecializationDTO>>>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenCacheMiss_QueriesDatabaseAndSetsCache()
        {
            var paged = new PaginatedResponse<ResponseDoctorSpecializationDTO>
            {
                Data = new List<ResponseDoctorSpecializationDTO> { new() { Id = 1, Name = "Cardiology" } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _cacheMock.Setup(c => c.Get<PaginatedResponse<ResponseDoctorSpecializationDTO>>(It.IsAny<string>())).Returns((PaginatedResponse<ResponseDoctorSpecializationDTO>?)null);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<DoctorSpecialization, ResponseDoctorSpecializationDTO>>>())).ReturnsAsync(paged);

            var handler = new GetAllDoctorSpecializationsHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetAllDoctorSpecializationsQuery(new PaginationRequest()), CancellationToken.None);

            result.Should().Be(paged);
            _cacheMock.Verify(c => c.Set(It.IsAny<string>(), paged, null), Times.Once);
        }

        // --- GetById ---
        [Fact]
        public async Task GetById_WhenCacheHit_ReturnsCachedResult()
        {
            var cached = new ResponseDoctorSpecializationDTO { Id = 1, Name = "Cardiology" };
            _cacheMock.Setup(c => c.Get<ResponseDoctorSpecializationDTO>(It.IsAny<string>())).Returns(cached);

            var handler = new GetDoctorSpecializationByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetDoctorSpecializationByIdQuery(1), CancellationToken.None);

            result.Should().Be(cached);
            _unitOfWorkMock.Verify(u => u.DoctorSpecializations.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenCacheMiss_QueriesDatabaseAndSetsCache()
        {
            var specialization = new DoctorSpecialization { Id = 1, Name = "Cardiology" };
            var dto = new ResponseDoctorSpecializationDTO { Id = 1, Name = "Cardiology" };

            _cacheMock.Setup(c => c.Get<ResponseDoctorSpecializationDTO>(It.IsAny<string>())).Returns((ResponseDoctorSpecializationDTO?)null);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _mapperMock.Setup(m => m.Map<ResponseDoctorSpecializationDTO>(specialization)).Returns(dto);

            var handler = new GetDoctorSpecializationByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new GetDoctorSpecializationByIdQuery(1), CancellationToken.None);

            result.Should().Be(dto);
            _cacheMock.Verify(c => c.Set(It.IsAny<string>(), dto, null), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            _cacheMock.Setup(c => c.Get<ResponseDoctorSpecializationDTO>(It.IsAny<string>())).Returns((ResponseDoctorSpecializationDTO?)null);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(99)).ReturnsAsync((DoctorSpecialization?)null);

            var handler = new GetDoctorSpecializationByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new GetDoctorSpecializationByIdQuery(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // --- Create ---
        [Fact]
        public async Task Create_WhenNameIsUnique_InvalidatesCacheAndReturnsDTO()
        {
            var dto = new CreateDoctorSpecializationDTO { Name = "Neurology" };
            var entity = new DoctorSpecialization { Id = 1, Name = "Neurology" };
            var response = new ResponseDoctorSpecializationDTO { Id = 1, Name = "Neurology" };

            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<DoctorSpecialization>(dto)).Returns(entity);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AddAsync(entity)).ReturnsAsync(entity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ResponseDoctorSpecializationDTO>(entity)).Returns(response);

            var handler = new CreateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new CreateDoctorSpecializationCommand(dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("Neurology");
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.DoctorSpecialization), Times.Once);
        }

        [Fact]
        public async Task Create_WhenNameIsDuplicate_ThrowsDuplicateException()
        {
            var dto = new CreateDoctorSpecializationDTO { Name = "Cardiology" };
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(true);

            var handler = new CreateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new CreateDoctorSpecializationCommand(dto), CancellationToken.None);

            await act.Should().ThrowAsync<DuplicateException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }

        // --- Update ---
        [Fact]
        public async Task Update_WhenSpecializationExists_InvalidatesCacheAndReturnsDTO()
        {
            var dto = new CreateDoctorSpecializationDTO { Name = "Updated" };
            var specialization = new DoctorSpecialization { Id = 1, Name = "Old" };
            var response = new ResponseDoctorSpecializationDTO { Id = 1, Name = "Updated" };

            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map(dto, specialization));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ResponseDoctorSpecializationDTO>(specialization)).Returns(response);

            var handler = new UpdateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new UpdateDoctorSpecializationCommand(1, dto), CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("Updated");
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.DoctorSpecialization), Times.Once);
        }

        [Fact]
        public async Task Update_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateDoctorSpecializationDTO { Name = "Updated" };
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(99)).ReturnsAsync((DoctorSpecialization?)null);

            var handler = new UpdateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new UpdateDoctorSpecializationCommand(99, dto), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }

        // --- Delete ---
        [Fact]
        public async Task Delete_WhenSpecializationExists_InvalidatesCacheAndReturnsTrue()
        {
            var specialization = new DoctorSpecialization { Id = 1, Name = "Cardiology" };
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteDoctorSpecializationHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var result = await handler.Handle(new DeleteDoctorSpecializationCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            _cacheMock.Verify(c => c.RemoveByPrefix(CacheKeys.DoctorSpecialization), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(99)).ReturnsAsync((DoctorSpecialization?)null);

            var handler = new DeleteDoctorSpecializationHandler(_unitOfWorkMock.Object, _cacheMock.Object);
            var act = async () => await handler.Handle(new DeleteDoctorSpecializationCommand(99), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _cacheMock.Verify(c => c.RemoveByPrefix(It.IsAny<string>()), Times.Never);
        }
    }
}
