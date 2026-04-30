using Microsoft.EntityFrameworkCore;
using MyStore.Api.Models.Dtos;
using MyStore.Api.Models.Entities;
using MyStore.Api.Repositories;

namespace MyStore.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo) => _repo = repo;

    public async Task<List<CategoryDto>> ListAsync()
    {
        return await _repo.Query()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        return c is null ? null : ToDto(c);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var slug = dto.Slug.Trim().ToLowerInvariant();
        if (await _repo.AnyAsync(c => c.Slug == slug))
            throw new InvalidOperationException("Slug already in use.");

        var entity = new Category
        {
            Name = dto.Name.Trim(),
            Slug = slug,
            Description = dto.Description
        };
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return null;

        entity.Name = dto.Name.Trim();
        entity.Slug = dto.Slug.Trim().ToLowerInvariant();
        entity.Description = dto.Description;
        _repo.Update(entity);
        await _repo.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return false;
        _repo.Remove(entity);
        await _repo.SaveChangesAsync();
        return true;
    }

    private static CategoryDto ToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description
    };
}
