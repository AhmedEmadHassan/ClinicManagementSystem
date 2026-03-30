namespace ClinicManagementSystem.Application.Services.Abstraction.GenericInterface
{
    public interface IGenericService<TResponseDto, TCreateDto, TUpdateDto>
    {
        Task<List<TResponseDto>> GetAll();
        Task<TResponseDto> GetById(int id);
        Task<TResponseDto> Create(TCreateDto dto);
        Task<TResponseDto> Update(int id, TUpdateDto dto);
        Task<bool> Delete(int id);
    }
    public interface IGenericService<TResponseDto, TCreateDto>
    : IGenericService<TResponseDto, TCreateDto, TCreateDto>
    { }
}
