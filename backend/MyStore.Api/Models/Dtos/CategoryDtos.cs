using System.ComponentModel.DataAnnotations;

namespace MyStore.Api.Models.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductCount { get; set; }
}

public class CreateCategoryDto
{
    [Required, MaxLength(80)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(80)] public string Slug { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
}
