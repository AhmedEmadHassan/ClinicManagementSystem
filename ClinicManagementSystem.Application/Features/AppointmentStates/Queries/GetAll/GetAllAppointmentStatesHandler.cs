using AutoMapper;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.AppointmentStates.Queries.GetAll
{
    public record GetAllAppointmentStatesQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseAppointmentStateDTO>>;

    public class GetAllAppointmentStatesHandler
        : IRequestHandler<GetAllAppointmentStatesQuery, PaginatedResponse<ResponseAppointmentStateDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllAppointmentStatesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<ResponseAppointmentStateDTO>> Handle(
            GetAllAppointmentStatesQuery request, CancellationToken cancellationToken)
        {
            var paged = await _unitOfWork.AppointmentStates.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                s => new ResponseAppointmentStateDTO
                {
                    Id = s.Id,
                    Name = s.Name
                });

            return paged;
        }
    }
}
