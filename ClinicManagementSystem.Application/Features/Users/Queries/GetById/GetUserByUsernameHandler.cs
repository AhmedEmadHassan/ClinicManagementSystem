using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Application.Features.Users.Queries.GetById
{
    public record GetUserByUsernameQuery(string UserName) : IRequest<ResponseUserDTO>;

    public class GetUserByUsernameHandler : IRequestHandler<GetUserByUsernameQuery, ResponseUserDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByUsernameHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResponseUserDTO> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
                throw new NotFoundException("User", request.UserName, "Username");

            var roles = await _userManager.GetRolesAsync(user);

            return new ResponseUserDTO
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty,
                DoctorId = user.DoctorId,
                PatientId = user.PatientId
            };
        }
    }
}
