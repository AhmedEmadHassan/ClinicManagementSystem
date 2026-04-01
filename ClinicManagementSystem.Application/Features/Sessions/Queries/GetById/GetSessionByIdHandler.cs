using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Sessions.Queries.GetById
{
    // Queries/GetById
    public record GetSessionByIdQuery(int Id) : IRequest<ResponseSessionDTO>;

    public class GetSessionByIdHandler : IRequestHandler<GetSessionByIdQuery, ResponseSessionDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSessionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseSessionDTO> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(request.Id);

            if (session is null)
                throw new NotFoundException(nameof(Session), request.Id);

            var patient = await _unitOfWork.Patients.GetByIdAsync(session.PatientId);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(session.DoctorId);

            session.Patient = patient;
            session.Doctor = doctor;

            return _mapper.Map<ResponseSessionDTO>(session);
        }
    }
}
