using AutoMapper;
using ClinicManagementSystem.Application.DTOs.CreateDTOs;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services.Implementation
{
    public class AppointmentStateService : IAppointmentStateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentStateService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponseAppointmentStateDTO>> GetAll()
        {
            var states = await _unitOfWork.AppointmentStates.GetAllAsync();
            return _mapper.Map<List<ResponseAppointmentStateDTO>>(states);
        }

        public async Task<ResponseAppointmentStateDTO> GetById(int id)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), id);

            return _mapper.Map<ResponseAppointmentStateDTO>(state);
        }

        public async Task<ResponseAppointmentStateDTO> Create(CreateAppointmentStateDTO dto)
        {
            var exists = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Name == dto.Name);

            if (exists)
                throw new DuplicateException($"AppointmentState with name '{dto.Name}' already exists.");

            var entity = _mapper.Map<AppointmentState>(dto);

            await _unitOfWork.AppointmentStates.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponseAppointmentStateDTO>(entity);
        }

        public async Task<ResponseAppointmentStateDTO> Update(int id, CreateAppointmentStateDTO dto)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), id);

            var duplicate = await _unitOfWork.AppointmentStates.AnyAsync(s => s.Name == dto.Name && s.Id != id);

            if (duplicate)
                throw new DuplicateException($"AppointmentState with name '{dto.Name}' already exists.");

            _mapper.Map(dto, state);

            await _unitOfWork.AppointmentStates.UpdateAsync(state);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ResponseAppointmentStateDTO>(state);
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
