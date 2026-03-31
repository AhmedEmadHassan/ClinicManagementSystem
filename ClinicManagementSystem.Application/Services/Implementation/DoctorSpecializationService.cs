using AutoMapper;
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
        private readonly IMapper _mapper;

        public DoctorSpecializationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponseDoctorSpecializationDTO>> GetAll()
        {
            var specializations = await _unitOfWork.DoctorSpecializations.GetAllAsync();
            return _mapper.Map<List<ResponseDoctorSpecializationDTO>>(specializations);
        }

        public async Task<ResponseDoctorSpecializationDTO> GetById(int id)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), id);

            return _mapper.Map<ResponseDoctorSpecializationDTO>(specialization);
        }

        public async Task<ResponseDoctorSpecializationDTO> Create(CreateDoctorSpecializationDTO dto)
        {
            var exists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Name == dto.Name);

            if (exists)
                throw new DuplicateException($"DoctorSpecialization with name '{dto.Name}' already exists.");

            var entity = _mapper.Map<DoctorSpecialization>(dto);

            await _unitOfWork.DoctorSpecializations.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponseDoctorSpecializationDTO>(entity);
        }

        public async Task<ResponseDoctorSpecializationDTO> Update(int id, CreateDoctorSpecializationDTO dto)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), id);

            var duplicate = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Name == dto.Name && s.Id != id);

            if (duplicate)
                throw new DuplicateException($"DoctorSpecialization with name '{dto.Name}' already exists.");

            _mapper.Map(dto, specialization);

            await _unitOfWork.DoctorSpecializations.UpdateAsync(specialization);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponseDoctorSpecializationDTO>(specialization);
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
