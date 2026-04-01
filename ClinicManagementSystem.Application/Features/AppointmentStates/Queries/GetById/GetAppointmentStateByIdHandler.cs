using AutoMapper;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetById
{
    // Queries/GetById
    public record GetAppointmentStateByIdQuery(int Id) : IRequest<ResponseAppointmentStateDTO>;

    public class GetAppointmentStateByIdHandler : IRequestHandler<GetAppointmentStateByIdQuery, ResponseAppointmentStateDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAppointmentStateByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseAppointmentStateDTO> Handle(GetAppointmentStateByIdQuery request, CancellationToken cancellationToken)
        {
            var state = await _unitOfWork.AppointmentStates.GetByIdAsync(request.Id);

            if (state is null)
                throw new NotFoundException(nameof(AppointmentState), request.Id);

            return _mapper.Map<ResponseAppointmentStateDTO>(state);
        }
    }
}
