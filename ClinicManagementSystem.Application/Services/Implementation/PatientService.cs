using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponsePatientDTO>> GetAll()
        {
            return await _unitOfWork.Patients.GetAllAsync(p => new ResponsePatientDTO
            {
                Id = p.Id,
                Name = p.Name,
                Phone = p.Phone,
                Gender = p.Gender ? "Male" : "Female",
                Email = p.Email,
                Address = p.Address,
                DateOfBirth = p.DateOfBirth,
                Summary = p.Summary
            });
        }

        public async Task<ResponsePatientDTO> GetById(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), id);

            return new ResponsePatientDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                Phone = patient.Phone,
                Gender = MapGender(patient.Gender),
                Email = patient.Email,
                Address = patient.Address,
                DateOfBirth = patient.DateOfBirth,
                Summary = patient.Summary
            };
        }

        public async Task<ResponsePatientDTO> Create(CreatePatientDTO dto)
        {
            var entity = new Patient
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Gender = ParseGender(dto.Gender),
                Email = dto.Email,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                Summary = dto.Summary
            };

            await _unitOfWork.Patients.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePatientDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Phone = entity.Phone,
                Gender = MapGender(entity.Gender),
                Email = entity.Email,
                Address = entity.Address,
                DateOfBirth = entity.DateOfBirth,
                Summary = entity.Summary
            };
        }

        public async Task<ResponsePatientDTO> Update(int id, CreatePatientDTO dto)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), id);

            patient.Name = dto.Name;
            patient.Phone = dto.Phone;
            patient.Gender = ParseGender(dto.Gender);
            patient.Email = dto.Email;
            patient.Address = dto.Address;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Summary = dto.Summary;

            await _unitOfWork.Patients.UpdateAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePatientDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                Phone = patient.Phone,
                Gender = MapGender(patient.Gender),
                Email = patient.Email,
                Address = patient.Address,
                DateOfBirth = patient.DateOfBirth,
                Summary = patient.Summary
            };
        }

        public async Task<bool> Delete(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), id);

            await _unitOfWork.Patients.DeleteAsync(patient);
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
