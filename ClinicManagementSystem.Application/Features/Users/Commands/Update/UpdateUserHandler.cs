using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Domain.Entities.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicManagementSystem.Application.Features.Users.Commands.Update
{
    // Commands/Update
    public record UpdateUserCommand(string Id, UpdateUserDTO Dto) : IRequest<ResponseUserDTO>;

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ResponseUserDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResponseUserDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user is null)
                throw new NotFoundException("User", 0);

            var existingUser = await _userManager.FindByNameAsync(request.Dto.UserName);
            if (existingUser is not null && existingUser.Id != request.Id)
                throw new DuplicateException($"Username '{request.Dto.UserName}' is already taken.");

            user.UserName = request.Dto.UserName;
            user.Email = request.Dto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

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

    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Dto.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");

            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
