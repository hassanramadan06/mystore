using System.ComponentModel.DataAnnotations;

namespace MyStore.Api.Models.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
}

public class CreateProductDto
{
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [MaxLength(2000)] public string Description { get; set; } = string.Empty;
    [Range(0, 1_000_000)] public decimal Price { get; set; }
    [Range(0, 100_000)] public int Stock { get; set; }
    [MaxLength(500)] public string ImageUrl { get; set; } = string.Empty;
    [MaxLength(80)] public string? Brand { get; set; }
    public bool IsFeatured { get; set; }
    [Required] public int CategoryId { get; set; }
}

public class UpdateProductDto : CreateProductDto { }

public class ProductQuery
{
    public string? Search { get; set; }
    public string? Category { get; set; } // slug
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; } // price_asc | price_desc | newest | name
    public bool? Featured { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
