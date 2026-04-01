using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Doctors.Queries.GetById
{
    // Queries/GetById
    public record GetDoctorByIdQuery(int Id) : IRequest<ResponseDoctorDTO>;

    public class GetDoctorByIdHandler : IRequestHandler<GetDoctorByIdQuery, ResponseDoctorDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDoctorByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDoctorDTO> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.Id);

            if (doctor is null)
                throw new NotFoundException(nameof(Doctor), request.Id);

            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(doctor.DoctorSpecializationId);
            doctor.DoctorSpecialization = specialization;

            return _mapper.Map<ResponseDoctorDTO>(doctor);
        }
    }
}
