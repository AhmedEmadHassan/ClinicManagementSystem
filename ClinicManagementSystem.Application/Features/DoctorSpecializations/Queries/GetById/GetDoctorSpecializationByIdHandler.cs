using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetById
{
    // Queries/GetById
    public record GetDoctorSpecializationByIdQuery(int Id) : IRequest<ResponseDoctorSpecializationDTO>;

    public class GetDoctorSpecializationByIdHandler : IRequestHandler<GetDoctorSpecializationByIdQuery, ResponseDoctorSpecializationDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDoctorSpecializationByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDoctorSpecializationDTO> Handle(GetDoctorSpecializationByIdQuery request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.DoctorSpecializations.GetByIdAsync(request.Id);

            if (specialization is null)
                throw new NotFoundException(nameof(DoctorSpecialization), request.Id);

            return _mapper.Map<ResponseDoctorSpecializationDTO>(specialization);
        }
    }
}
