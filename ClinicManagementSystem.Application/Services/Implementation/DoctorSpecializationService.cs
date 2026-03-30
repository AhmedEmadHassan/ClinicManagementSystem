using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services.Implementation
{
    public class DoctorSpecializationService : IDoctorSpecializationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorSpecializationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseDoctorSpecializationDTO>> GetAll()
        {
            var specializations = await _unitOfWork.DoctorSpecializations.GetAllAsync();

            return specializations.Select(s => new ResponseDoctorSpecializationDTO
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public async Task<ResponseDoctorSpecializationDTO> GetById(int id)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), id);

            return new ResponseDoctorSpecializationDTO
            {
                Id = specialization.Id,
                Name = specialization.Name
            };
        }

        public async Task<ResponseDoctorSpecializationDTO> Create(CreateDoctorSpecializationDTO dto)
        {
            var exists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Name == dto.Name);

            if (exists)
                throw new DuplicateException($"DoctorSpecialization with name '{dto.Name}' already exists.");

            var entity = new DoctorSpecialization
            {
                Name = dto.Name
            };

            await _unitOfWork.DoctorSpecializations.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDoctorSpecializationDTO
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public async Task<ResponseDoctorSpecializationDTO> Update(int id, CreateDoctorSpecializationDTO dto)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), id);

            var duplicate = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Name == dto.Name && s.Id != id);

            if (duplicate)
                throw new DuplicateException($"DoctorSpecialization with name '{dto.Name}' already exists.");

            specialization.Name = dto.Name;

            await _unitOfWork.DoctorSpecializations.UpdateAsync(specialization);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDoctorSpecializationDTO
            {
                Id = specialization.Id,
                Name = specialization.Name
            };
        }

        public async Task<bool> Delete(int id)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), id);

            await _unitOfWork.DoctorSpecializations.DeleteAsync(specialization);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
