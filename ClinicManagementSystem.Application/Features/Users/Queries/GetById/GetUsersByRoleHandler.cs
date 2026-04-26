using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Application.Features.Users.Queries.GetById
{
    // Queries/GetByRole
    public record GetUsersByRoleQuery(string Role) : IRequest<List<ResponseUserDTO>>;

    public class GetUsersByRoleHandler : IRequestHandler<GetUsersByRoleQuery, List<ResponseUserDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUsersByRoleHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ResponseUserDTO>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
        {
            var validRoles = new[] { "Admin", "Doctor", "Patient", "Receptionist" };
            if (!validRoles.Contains(request.Role))
                throw new BadRequestException($"Invalid role '{request.Role}'. Accepted values are: {string.Join(", ", validRoles)}.");

            var users = await _userManager.GetUsersInRoleAsync(request.Role);

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

            return data;
        }
    }
}
