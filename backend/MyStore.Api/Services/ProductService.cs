using Microsoft.EntityFrameworkCore;
using MyStore.Api.Models.Dtos;
using MyStore.Api.Models.Entities;
using MyStore.Api.Repositories;

namespace MyStore.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public ProductService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<PagedResult<ProductDto>> SearchAsync(ProductQuery q)
    {
        var query = _products.QueryWithCategory();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(p =>
                p.Name.Contains(s) ||
                (p.Brand != null && p.Brand.Contains(s)) ||
                p.Description.Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(q.Category))
        {
            var slug = q.Category.Trim().ToLower();
            query = query.Where(p => p.Category!.Slug == slug);
        }

        if (q.MinPrice.HasValue) query = query.Where(p => p.Price >= q.MinPrice.Value);
        if (q.MaxPrice.HasValue) query = query.Where(p => p.Price <= q.MaxPrice.Value);
        if (q.Featured == true) query = query.Where(p => p.IsFeatured);

        query = q.Sort switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name" => query.OrderBy(p => p.Name),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync();
        var page = Math.Max(1, q.Page);
        var size = Math.Clamp(q.PageSize, 1, 100);

        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = items.Select(ToDto).ToList(),
            Total = total,
            Page = page,
            PageSize = size
        };
    }

    public async Task<ProductDto?> GetAsync(int id)
    {
        var p = await _products.GetWithCategoryAsync(id);
        return p is null ? null : ToDto(p);
    }

    public async Task<List<ProductDto>> GetFeaturedAsync(int take = 8)
    {
        var list = await _products.QueryWithCategory()
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync();
        return list.Select(ToDto).ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        if (!await _categories.AnyAsync(c => c.Id == dto.CategoryId))
            throw new InvalidOperationException("Category does not exist.");

        var entity = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            Brand = dto.Brand,
            IsFeatured = dto.IsFeatured,
            CategoryId = dto.CategoryId
        };
        await _products.AddAsync(entity);
        await _products.SaveChangesAsync();

        var saved = await _products.GetWithCategoryAsync(entity.Id);
        return ToDto(saved!);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var entity = await _products.GetByIdAsync(id);
        if (entity is null) return null;

        if (!await _categories.AnyAsync(c => c.Id == dto.CategoryId))
            throw new InvalidOperationException("Category does not exist.");

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.Stock = dto.Stock;
        entity.ImageUrl = dto.ImageUrl;
        entity.Brand = dto.Brand;
        entity.IsFeatured = dto.IsFeatured;
        entity.CategoryId = dto.CategoryId;

        _products.Update(entity);
        await _products.SaveChangesAsync();

        var saved = await _products.GetWithCategoryAsync(entity.Id);
        return ToDto(saved!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _products.GetByIdAsync(id);
        if (entity is null) return false;
        _products.Remove(entity);
        await _products.SaveChangesAsync();
        return true;
    }

    private static ProductDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Stock = p.Stock,
        ImageUrl = p.ImageUrl,
        Brand = p.Brand,
        IsFeatured = p.IsFeatured,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty,
        CategorySlug = p.Category?.Slug ?? string.Empty
    };
}
