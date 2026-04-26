using ClinicManagementSystem.Application.Common.Pagination;
using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.DTOs.UpdateDTOs;

namespace ClinicManagementSystem.Application.Services.Abstraction
{
    public interface IUserService
    {
        Task<PaginatedResponse<ResponseUserDTO>> GetAll(PaginationRequest pagination);
        Task<ResponseUserDTO> GetById(string id);
        Task<ResponseUserDTO> Update(string id, UpdateUserDTO dto);
        Task<bool> Delete(string id);
        Task<ResponseUserDTO> ChangeRole(string id, ChangeUserRoleDTO dto);
        Task<List<ResponseUserDTO>> GetByRole(string role);
    }
}
