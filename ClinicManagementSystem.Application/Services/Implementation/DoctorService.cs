using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services.Implementation
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponseDoctorDTO>> GetAll()
        {
            return await _unitOfWork.Doctors.GetAllAsync(d => new ResponseDoctorDTO
            {
                Id = d.Id,
                Name = d.Name,
                Phone = d.Phone,
                Gender = d.Gender ? "Male" : "Female",
                Email = d.Email,
                Address = d.Address,
                DateOfBirth = d.DateOfBirth,
                Summary = d.Summary,
                DoctorSpecializationId = d.DoctorSpecializationId,
                DoctorSpecializationName = d.DoctorSpecialization.Name
            });
        }

        public async Task<ResponseDoctorDTO> GetById(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);

            if (doctor is null)
                throw new NotFoundException(nameof(Doctor), id);

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(doctor.DoctorSpecializationId);

            return _mapper.Map<ResponseDoctorDTO>(doctor);
        }

        public async Task<ResponseDoctorDTO> Create(CreateDoctorDTO dto)
        {
            var specializationExists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Id == dto.DoctorSpecializationId);

            if (!specializationExists)
                throw new NotFoundException(nameof(DoctorSpecialization), dto.DoctorSpecializationId);

            var entity = _mapper.Map<Doctor>(dto);

            await _unitOfWork.Doctors.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(entity.DoctorSpecializationId);
            entity.DoctorSpecialization = specialization;

            return _mapper.Map<ResponseDoctorDTO>(entity);
        }

        public async Task<ResponseDoctorDTO> Update(int id, CreateDoctorDTO dto)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);

            if (doctor is null)
                throw new NotFoundException(nameof(Doctor), id);

            var specializationExists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Id == dto.DoctorSpecializationId);

            if (!specializationExists)
                throw new NotFoundException(nameof(DoctorSpecialization), dto.DoctorSpecializationId);

            _mapper.Map(dto, doctor);

            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(doctor.DoctorSpecializationId);
            doctor.DoctorSpecialization = specialization;

            return _mapper.Map<ResponseDoctorDTO>(doctor);
        }

        public async Task<bool> Delete(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);

            if (doctor is null)
                throw new NotFoundException(nameof(Doctor), id);

            await _unitOfWork.Doctors.DeleteAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
