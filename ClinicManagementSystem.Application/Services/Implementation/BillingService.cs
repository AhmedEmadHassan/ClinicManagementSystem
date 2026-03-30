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

        public BillingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseBillingDTO>> GetAll()
        {
            var billings = await _unitOfWork.Billings.GetAllAsync();
            var patients = await _unitOfWork.Patients.GetAllAsync();

            var patientMap = patients.ToDictionary(p => p.Id, p => p.Name);

            return billings.Select(b => new ResponseBillingDTO
            {
                Id = b.Id,
                SessionId = b.SessionId,
                PatientId = b.PatientId,
                Description = b.Description,
                Amount = b.Amount,
                IsPaid = b.IsPaid,
                PatientName = patientMap.TryGetValue(b.PatientId, out var patientName) ? patientName : string.Empty
            }).ToList();
        }

        public async Task<ResponseBillingDTO> GetById(int id)
        {
            var billing = await _unitOfWork.Billings.GetByIdAsync(id);

            if (billing is null)
                throw new NotFoundException(nameof(Billing), id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);

            return new ResponseBillingDTO
            {
                Id = billing.Id,
                SessionId = billing.SessionId,
                PatientId = billing.PatientId,
                Description = billing.Description,
                Amount = billing.Amount,
                IsPaid = billing.IsPaid,
                PatientName = patient?.Name ?? string.Empty
            };
        }

        public async Task<ResponseBillingDTO> Create(CreateBillingDTO dto)
        {
            var sessionExists = await _unitOfWork.Sessions.AnyAsync(s => s.Id == dto.SessionId);
            if (!sessionExists)
                throw new NotFoundException(nameof(Session), dto.SessionId);

            var patientExists = await _unitOfWork.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                throw new NotFoundException(nameof(Patient), dto.PatientId);

            var entity = new Billing
            {
                SessionId = dto.SessionId,
                PatientId = dto.PatientId,
                Description = dto.Description,
                Amount = dto.Amount,
                IsPaid = dto.IsPaid
            };

            await _unitOfWork.Billings.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(entity.PatientId);

            return new ResponseBillingDTO
            {
                Id = entity.Id,
                SessionId = entity.SessionId,
                PatientId = entity.PatientId,
                Description = entity.Description,
                Amount = entity.Amount,
                IsPaid = entity.IsPaid,
                PatientName = patient?.Name ?? string.Empty
            };
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

            billing.SessionId = dto.SessionId;
            billing.PatientId = dto.PatientId;
            billing.Description = dto.Description;
            billing.Amount = dto.Amount;
            billing.IsPaid = dto.IsPaid;

            await _unitOfWork.Billings.UpdateAsync(billing);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _unitOfWork.Patients.GetByIdAsync(billing.PatientId);

            return new ResponseBillingDTO
            {
                Id = billing.Id,
                SessionId = billing.SessionId,
                PatientId = billing.PatientId,
                Description = billing.Description,
                Amount = billing.Amount,
                IsPaid = billing.IsPaid,
                PatientName = patient?.Name ?? string.Empty
            };
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
