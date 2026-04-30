using MyStore.Api.Models.Dtos;

namespace MyStore.Api.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> SearchAsync(ProductQuery query);
    Task<ProductDto?> GetAsync(int id);
    Task<List<ProductDto>> GetFeaturedAsync(int take = 8);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}
