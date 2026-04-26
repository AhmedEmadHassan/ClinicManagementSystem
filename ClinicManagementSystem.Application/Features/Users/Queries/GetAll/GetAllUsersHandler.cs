using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Application.Features.Users.Queries.GetAll
{
    public record GetAllUsersQuery(PaginationRequest Pagination) : IRequest<PaginatedResponse<ResponseUserDTO>>;

    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, PaginatedResponse<ResponseUserDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PaginatedResponse<ResponseUserDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = _userManager.Users
                .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize)
                .ToList();

            var totalCount = _userManager.Users.Count();

            var data = new List<ResponseUserDTO>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                data.Add(new ResponseUserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    DoctorId = user.DoctorId,
                    PatientId = user.PatientId
                });
            }

            return new PaginatedResponse<ResponseUserDTO>
            {
                Data = data,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
