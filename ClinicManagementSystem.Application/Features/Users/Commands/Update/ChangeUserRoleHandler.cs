using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Domain.Entities.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagementSystem.Application.Features.Users.Commands.Update
{
    // Commands/ChangeRole
    public record ChangeUserRoleCommand(string Id, ChangeUserRoleDTO Dto) : IRequest<ResponseUserDTO>;

    public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand, ResponseUserDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangeUserRoleHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResponseUserDTO> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user is null)
                throw new NotFoundException("User", 0);

            var validRoles = new[] { "Admin", "Doctor", "Patient", "Receptionist" };
            if (!validRoles.Contains(request.Dto.Role))
                throw new BadRequestException($"Invalid role '{request.Dto.Role}'. Accepted values are: {string.Join(", ", validRoles)}.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Dto.Role);

            return new ResponseUserDTO
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = request.Dto.Role,
                DoctorId = user.DoctorId,
                PatientId = user.PatientId
            };
        }
    }

    public class ChangeUserRoleValidator : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleValidator()
        {
            RuleFor(x => x.Dto.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(r => new[] { "Admin", "Doctor", "Patient", "Receptionist" }.Contains(r))
                .WithMessage("Role must be one of: Admin, Doctor, Patient, Receptionist.");
        }
    }
}
