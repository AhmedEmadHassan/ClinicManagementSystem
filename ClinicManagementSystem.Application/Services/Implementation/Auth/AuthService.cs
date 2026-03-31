using ClinicManagementSystem.Application.DTOs.AuthDTOs;
using ClinicManagementSystem.Application.Exceptions;
using ClinicManagementSystem.Application.Mapping.Helpers;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Application.Services.Abstraction.Auth;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ClinicManagementSystem.Application.Services.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDTO> Register(RegisterDTO dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser is not null)
                throw new DuplicateException($"Username '{dto.UserName}' is already taken.");

            var validRoles = new[] { "Admin", "Doctor", "Patient", "Receptionist" };
            if (!validRoles.Contains(dto.Role))
                throw new BadRequestException($"Invalid role '{dto.Role}'. Accepted values are: {string.Join(", ", validRoles)}.");

            if (dto.Role == "Doctor" && dto.DoctorSpecializationId is null)
                throw new BadRequestException("DoctorSpecializationId is required when registering a Doctor.");

            if ((dto.Role == "Doctor" || dto.Role == "Patient") &&
                (string.IsNullOrWhiteSpace(dto.Phone) || string.IsNullOrWhiteSpace(dto.Gender)))
                throw new BadRequestException("Phone and Gender are required when registering a Doctor or Patient.");

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, dto.Role);

            if (dto.Role == "Doctor")
            {
                var specializationExists = await _unitOfWork.DoctorSpecializations.AnyAsync(s => s.Id == dto.DoctorSpecializationId);
                if (!specializationExists)
                    throw new NotFoundException(nameof(DoctorSpecialization), dto.DoctorSpecializationId!.Value);

                var doctor = new Doctor
                {
                    Name = dto.UserName,
                    Phone = dto.Phone!,
                    Gender = GenderHelper.Parse(dto.Gender!),
                    DateOfBirth = dto.DateOfBirth,
                    Address = dto.Address,
                    Summary = dto.Summary,
                    DoctorSpecializationId = dto.DoctorSpecializationId!.Value
                };

                await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();

                user.DoctorId = doctor.Id;
                await _userManager.UpdateAsync(user);
            }
            else if (dto.Role == "Patient")
            {
                var patient = new Patient
                {
                    Name = dto.UserName,
                    Phone = dto.Phone!,
                    Gender = GenderHelper.Parse(dto.Gender!),
                    DateOfBirth = dto.DateOfBirth,
                    Address = dto.Address,
                    Summary = dto.Summary
                };

                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();

                user.PatientId = patient.Id;
                await _userManager.UpdateAsync(user);
            }

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDTO> Login(LoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new BadRequestException("Invalid username or password.");

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDTO> RefreshToken(RefreshTokenDTO dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.AccessToken);
            var userName = principal.Identity?.Name;

            var user = await _userManager.FindByNameAsync(userName!);

            if (user is null ||
                user.RefreshToken != dto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new BadRequestException("Invalid or expired refresh token.");

            return await GenerateAuthResponse(user);
        }

        public async Task Logout(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                throw new NotFoundException("User", 0);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userManager.UpdateAsync(user);
        }

        public async Task ChangePassword(string userName, ChangePasswordDTO dto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                throw new NotFoundException("User", 0);

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // --- Helpers ---
        private async Task<AuthResponseDTO> GenerateAuthResponse(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;
            var accessToken = GenerateAccessToken(user, role);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserName = user.UserName!,
                Role = role,
                ExpiresAt = expiresAt
            };
        }

        private string GenerateAccessToken(ApplicationUser user, string role)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,           user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role,           role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new BadRequestException("Invalid access token.");

            return principal;
        }
    }
}
