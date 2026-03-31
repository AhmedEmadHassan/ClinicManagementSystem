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

        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            return new ResponseDoctorDTO
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender ? "Male" : "Female",
                Email = doctor.Email,
                Address = doctor.Address,
                DateOfBirth = doctor.DateOfBirth,
                Summary = doctor.Summary,
                DoctorSpecializationId = doctor.DoctorSpecializationId,
                DoctorSpecializationName = specialization?.Name ?? string.Empty
            };
        }

        public async Task<ResponseDoctorDTO> Create(CreateDoctorDTO dto)
        {
            var specializationExists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Id == dto.DoctorSpecializationId);

            if (!specializationExists)
                throw new NotFoundException(nameof(DoctorSpecialization), dto.DoctorSpecializationId);

            var entity = new Doctor
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Gender = ParseGender(dto.Gender),
                Email = dto.Email,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                Summary = dto.Summary,
                DoctorSpecializationId = dto.DoctorSpecializationId
            };

            await _unitOfWork.Doctors.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(entity.DoctorSpecializationId);

            return new ResponseDoctorDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Phone = entity.Phone,
                Gender = MapGender(entity.Gender),
                Email = entity.Email,
                Address = entity.Address,
                DateOfBirth = entity.DateOfBirth,
                Summary = entity.Summary,
                DoctorSpecializationId = entity.DoctorSpecializationId,
                DoctorSpecializationName = specialization?.Name ?? string.Empty
            };
        }

        public async Task<ResponseDoctorDTO> Update(int id, CreateDoctorDTO dto)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);

            if (doctor is null)
                throw new NotFoundException(nameof(Doctor), id);

            var specializationExists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Id == dto.DoctorSpecializationId);

            if (!specializationExists)
                throw new NotFoundException(nameof(DoctorSpecialization), dto.DoctorSpecializationId);

            doctor.Name = dto.Name;
            doctor.Phone = dto.Phone;
            doctor.Gender = ParseGender(dto.Gender);
            doctor.Email = dto.Email;
            doctor.Address = dto.Address;
            doctor.DateOfBirth = dto.DateOfBirth;
            doctor.Summary = dto.Summary;
            doctor.DoctorSpecializationId = dto.DoctorSpecializationId;

            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(doctor.DoctorSpecializationId);

            return new ResponseDoctorDTO
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = MapGender(doctor.Gender),
                Email = doctor.Email,
                Address = doctor.Address,
                DateOfBirth = doctor.DateOfBirth,
                Summary = doctor.Summary,
                DoctorSpecializationId = doctor.DoctorSpecializationId,
                DoctorSpecializationName = specialization?.Name ?? string.Empty
            };
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

        private static bool ParseGender(string gender) =>
            gender.Trim().ToLower() switch
            {
                "male" => true,
                "female" => false,
                _ => throw new BadRequestException($"Invalid gender value '{gender}'. Accepted values are 'Male' or 'Female'.")
            };

        private static string MapGender(bool gender) => gender ? "Male" : "Female";
    }
}
