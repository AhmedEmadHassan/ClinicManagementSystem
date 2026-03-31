using ClinicManagementSystem.Application.DTOs.AuthDTOs;

namespace ClinicManagementSystem.Application.Services.Abstraction.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Register(RegisterDTO dto);
        Task<AuthResponseDTO> Login(LoginDTO dto);
        Task<AuthResponseDTO> RefreshToken(RefreshTokenDTO dto);
        Task Logout(string userName);
        Task ChangePassword(string userName, ChangePasswordDTO dto);
    }
}
