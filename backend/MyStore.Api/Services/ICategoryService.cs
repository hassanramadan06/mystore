using MyStore.Api.Models.Dtos;

namespace MyStore.Api.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> ListAsync();
    Task<CategoryDto?> GetAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(int id, CreateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}
