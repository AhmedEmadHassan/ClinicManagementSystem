using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services.Implementation
{
    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BillingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponseBillingDTO>> GetAll()
        {
            return await _unitOfWork.Billings.GetAllAsync(b => new ResponseBillingDTO
            {
                Id = b.Id,
                SessionId = b.SessionId,
                PatientId = b.PatientId,
                Description = b.Description,
                Amount = b.Amount,
                IsPaid = b.IsPaid,
                PatientName = b.Patient.Name
            });
        }

        public async Task<ResponseBillingDTO> GetById(int id)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);
            billing.Patient = patient;

            return _mapper.Map<ResponseBillingDTO>(billing);
        }

        public async Task<ResponseBillingDTO> Create(CreateBillingDTO dto)
        {
            var sessionExists = await _unitOfWork.Sessions.AnyAsync(s => s.Id == dto.SessionId);
            if (!sessionExists)
                throw new NotFoundException(nameof(Session), dto.SessionId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), dto.PatientId);

            var entity = _mapper.Map<Billing>(dto);

            await _unitOfWork.Billings.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);
            entity.Patient = patient;

            return _mapper.Map<ResponseBillingDTO>(entity);
        }

        public async Task<ResponseBillingDTO> Update(int id, CreateBillingDTO dto)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), id);

            var sessionExists = await _unitOfWork.Sessions.AnyAsync(s => s.Id == dto.SessionId);
            if (!sessionExists)
                throw new NotFoundException(nameof(Session), dto.SessionId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), dto.PatientId);

            _mapper.Map(dto, billing);

            await _unitOfWork.Billings.UpdateAsync(billing);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);
            billing.Patient = patient;

            return _mapper.Map<ResponseBillingDTO>(billing);
        }

        public async Task<bool> Delete(int id)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), id);

            await _unitOfWork.Billings.DeleteAsync(billing);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
