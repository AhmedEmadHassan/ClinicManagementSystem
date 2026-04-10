using AutoMapper;
using ClinicManagementSystem.Application.Common.Cache;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Queries.GetById
{
    // Queries/GetById
    public record GetSessionByIdQuery(int Id) : IRequest<ResponseSessionDTO>;

    public class GetSessionByIdHandler
    : IRequestHandler<GetSessionByIdQuery, ResponseSessionDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetSessionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResponseSessionDTO> Handle(
            GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.GetById(CacheKeys.Session, request.Id);
            var cached = _cache.Get<ResponseSessionDTO>(cacheKey);

            if (cached is not null)
                return cached;

            var session = await _unitOfWork.Sessions.GetByIdAsync(request.Id);

            if (session is null)
                throw new NotFoundException(nameof(Session), request.Id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(session.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(session.DoctorId);

            session.Patient = patient;
            session.Doctor = doctor;

            var dto = _mapper.Map<ResponseSessionDTO>(session);
            _cache.Set(cacheKey, dto);
            return dto;
        }
    }
}
