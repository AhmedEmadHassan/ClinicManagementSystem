using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Create;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Delete;
using ClinicManagementSystem.Application.Features.DoctorSpecializations.Commands.Update;
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

        public DoctorSpecializationHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        // --- GetById ---
        [Fact]
        public async Task GetById_WhenSpecializationExists_ReturnsDTO()
        {
            // Arrange
            #region Ready Data
            var specializationEntity = new DoctorSpecialization { Id = 1, Name = "Dentist" };
            var specializationDto = new ResponseDoctorSpecializationDTO { Id = 1, Name = "Dentist" };
            int QueryId = 1;
            #endregion
            #region Setup Mocks
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(QueryId)).ReturnsAsync(specializationEntity);
            _mapperMock.Setup(m => m.Map<ResponseDoctorSpecializationDTO>(specializationEntity)).Returns(specializationDto);
            #endregion
            #region Create Test Object
            var Handler = new GetDoctorSpecializationByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            #endregion
            // Act
            #region GetResult
            var result = await Handler.Handle(new GetDoctorSpecializationByIdQuery(QueryId), CancellationToken.None);
            #endregion
            // Assert
            #region Assertions
            result.Should().NotBeNull();
            result.Id.Should().Be(specializationDto.Id);
            result.Name.Should().Be(specializationDto.Name);
            #endregion
        }

        [Fact]
        public async Task GetById_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            // Arrange
            #region Ready Data
            int QueryId = 99;
            #endregion
            #region Setup Mocks
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(QueryId)).ReturnsAsync((DoctorSpecialization?)null);
            #endregion
            #region Create Test Object
            var Handler = new GetDoctorSpecializationByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            #endregion
            // Act
            #region GetResult
            var act = async () => await Handler.Handle(new GetDoctorSpecializationByIdQuery(QueryId), CancellationToken.None);
            #endregion
            // Assert
            #region Assertions
            await act.Should().ThrowAsync<NotFoundException>();
            #endregion
        }

        // --- Create ---
        [Fact]
        public async Task Create_WhenNameIsUnique_ReturnsCreatedDTO()
        {
            // Arrange
            #region Ready Data
            int NewId = 2;
            var CreatingEntity = new DoctorSpecialization { Name = "Cardiology" };
            var CreatingDto = new CreateDoctorSpecializationDTO { Name = "Cardiology" };
            var ResponseDto = new ResponseDoctorSpecializationDTO { Id = NewId, Name = "Cardiology" };
            #endregion
            #region Setup Mocks
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AddAsync(It.IsAny<DoctorSpecialization>())).ReturnsAsync(CreatingEntity);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Callback(() => { CreatingEntity.Id = NewId; }).ReturnsAsync(1); // specify return value
            _mapperMock.Setup(m => m.Map<DoctorSpecialization>(CreatingDto)).Returns(CreatingEntity);
            _mapperMock.Setup(m => m.Map<ResponseDoctorSpecializationDTO>(It.IsAny<DoctorSpecialization>()))
                           .Returns<DoctorSpecialization>(src => new ResponseDoctorSpecializationDTO
                           {
                               Id = src.Id,      // uses current Id at runtime
                               Name = src.Name
                           });
            #endregion
            #region Create Test Object
            var Handler = new CreateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            #endregion
            // Act
            #region GetResult
            var result = await Handler.Handle(new CreateDoctorSpecializationCommand(CreatingDto), CancellationToken.None);
            #endregion
            // Assert
            #region Assertions
            result.Should().NotBeNull();
            result.Id.Should().Be(NewId);
            result.Name.Should().Be("Cardiology");
            #endregion
        }

        [Fact]
        public async Task Create_WhenNameIsDuplicate_ThrowsDuplicateException()
        {
            // Arrange
            #region Ready Data
            var CreateDto = new CreateDoctorSpecializationDTO { Name = "Dermatology" };
            #endregion
            #region Setup Mocks
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(true);
            #endregion
            #region Create Test Object
            var Handler = new CreateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            #endregion
            // Act
            #region GetResult
            var act = async () => await Handler.Handle(new CreateDoctorSpecializationCommand(CreateDto), CancellationToken.None);
            #endregion
            // Assert
            #region Assertions
            await act.Should().ThrowAsync<Exception>();
            #endregion
        }

        // --- Update ---
        [Fact]
        public async Task Update_WhenSpecializationExists_ReturnsUpdatedDTO()
        {
            // Arrange
            var dto = new CreateDoctorSpecializationDTO { Name = "Updated" };
            var specialization = new DoctorSpecialization { Id = 1, Name = "Old" };
            var response = new ResponseDoctorSpecializationDTO { Id = 1, Name = "Updated" };

            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.AnyAsync(It.IsAny<Expression<Func<DoctorSpecialization, bool>>>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map(dto, specialization));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ResponseDoctorSpecializationDTO>(specialization)).Returns(response);

            var handler = new UpdateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(new UpdateDoctorSpecializationCommand(1, dto), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task Update_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateDoctorSpecializationDTO { Name = "Updated" };
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(99)).ReturnsAsync((DoctorSpecialization?)null);

            var handler = new UpdateDoctorSpecializationHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var act = async () => await handler.Handle(new UpdateDoctorSpecializationCommand(99, dto), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        // --- Delete ---
        [Fact]
        public async Task Delete_WhenSpecializationExists_ReturnsTrue()
        {
            // Arrange
            var specialization = new DoctorSpecialization { Id = 1, Name = "Cardiology" };

            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(1)).ReturnsAsync(specialization);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new DeleteDoctorSpecializationHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(new DeleteDoctorSpecializationCommand(1), CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WhenSpecializationNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.DoctorSpecializations.GetByIdAsync(99)).ReturnsAsync((DoctorSpecialization?)null);

            var handler = new DeleteDoctorSpecializationHandler(_unitOfWorkMock.Object);

            // Act
            var act = async () => await handler.Handle(new DeleteDoctorSpecializationCommand(99), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
