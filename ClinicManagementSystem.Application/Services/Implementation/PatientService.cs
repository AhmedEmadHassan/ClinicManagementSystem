using AutoMapper;
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
        private readonly IMapper _mapper;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponsePatientDTO>> GetAll()
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            return _mapper.Map<List<ResponsePatientDTO>>(patients);
        }

        public async Task<ResponsePatientDTO> GetById(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), id);

            return _mapper.Map<ResponsePatientDTO>(patient);
        }

        public async Task<ResponsePatientDTO> Create(CreatePatientDTO dto)
        {
            var entity = _mapper.Map<Patient>(dto);

            await _unitOfWork.Patients.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponsePatientDTO>(entity);
        }

        public async Task<ResponsePatientDTO> Update(int id, CreatePatientDTO dto)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), id);

            _mapper.Map(dto, patient);

            await _unitOfWork.Patients.UpdateAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponsePatientDTO>(patient);
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
    }
}
