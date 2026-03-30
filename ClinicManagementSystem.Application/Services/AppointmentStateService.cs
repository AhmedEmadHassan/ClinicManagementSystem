using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services
{
    public class AppointmentStateService : IAppointmentStateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentStateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseAppointmentStateDTO>> GetAll()
        {
            var states = await _unitOfWork.AppointmentStates.GetAllAsync();

            return states.Select(s => new ResponseAppointmentStateDTO
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public async Task<ResponseAppointmentStateDTO> GetById(int id)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), id);

            return new ResponseAppointmentStateDTO
            {
                Id = state.Id,
                Name = state.Name
            };
        }

        public async Task<ResponseAppointmentStateDTO> Create(CreateAppointmentStateDTO dto)
        {
            var exists = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Name == dto.Name);

            if (exists)
                throw new DuplicateException($"AppointmentState with name '{dto.Name}' already exists.");

            var entity = new AppointmentState
            {
                Name = dto.Name
            };

            await _unitOfWork.AppointmentStates.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAppointmentStateDTO
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public async Task<ResponseAppointmentStateDTO> Update(int id, CreateAppointmentStateDTO dto)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), id);

            var duplicate = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Name == dto.Name && s.Id != id);

            if (duplicate)
                throw new DuplicateException($"AppointmentState with name '{dto.Name}' already exists.");

            state.Name = dto.Name;

            await _unitOfWork.AppointmentStates.UpdateAsync(state);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAppointmentStateDTO
            {
                Id = state.Id,
                Name = state.Name
            };
        }

        public async Task<bool> Delete(int id)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), id);

            await _unitOfWork.AppointmentStates.DeleteAsync(state);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
