using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Application.Features.Users.Queries.GetById
{
    // Queries/GetById
    public record GetUserByIdQuery(string Id) : IRequest<ResponseUserDTO>;

    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, ResponseUserDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByIdHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResponseUserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user is null)
                throw new NotFoundException("User", 0);

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
