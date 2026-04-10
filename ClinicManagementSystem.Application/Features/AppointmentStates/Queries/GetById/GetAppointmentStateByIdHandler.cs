using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetById
{
    // Queries/GetById
    public record GetAppointmentStateByIdQuery(int Id) : IRequest<ResponseAppointmentStateDTO>;

    public class GetAppointmentStateByIdHandler
    : IRequestHandler<GetAppointmentStateByIdQuery, ResponseAppointmentStateDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetAppointmentStateByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseAppointmentStateDTO> Handle(
            GetAppointmentStateByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetById(CacheKeys.AppointmentState, request.Id);
            var cached = _cache.Get<ResponseAppointmentStateDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(request.Id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), request.Id);

            var dto = _mapper.Map<ResponseAppointmentStateDTO>(state);
            _cache.Set(cacheKey, dto);
            return dto;
        }
    }
}
