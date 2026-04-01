using AutoMapper;
using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.DoctorSpecializations.Queries.GetAll
{
    public record GetAllDoctorSpecializationsQuery(PaginationRequest Pagination)
    : IRequest<PaginatedResponse<ResponseDoctorSpecializationDTO>>;

    public class GetAllDoctorSpecializationsHandler
        : IRequestHandler<GetAllDoctorSpecializationsQuery, PaginatedResponse<ResponseDoctorSpecializationDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllDoctorSpecializationsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<ResponseDoctorSpecializationDTO>> Handle(
            GetAllDoctorSpecializationsQuery request, CancellationToken cancellationToken)
        {
            var paged = await _unitOfWork.DoctorSpecializations.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                s => new ResponseDoctorSpecializationDTO
                {
                    Id = s.Id,
                    Name = s.Name
                });

            return paged;
        }
    }
}
