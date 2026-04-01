using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetAll
{
    // Queries/GetAll
    public record GetAllAppointmentStatesQuery : IRequest<List<ResponseAppointmentStateDTO>>;

    public class GetAllAppointmentStatesHandler : IRequestHandler<GetAllAppointmentStatesQuery, List<ResponseAppointmentStateDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllAppointmentStatesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ResponseAppointmentStateDTO>> Handle(GetAllAppointmentStatesQuery request, CancellationToken cancellationToken)
        {
            var states = await _unitOfWork.AppointmentStates.GetAllAsync();
            return _mapper.Map<List<ResponseAppointmentStateDTO>>(states);
        }
    }
}
