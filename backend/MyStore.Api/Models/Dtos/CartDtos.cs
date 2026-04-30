using System.ComponentModel.DataAnnotations;

namespace MyStore.Api.Models.Dtos;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int Stock { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal => Items.Sum(i => i.LineTotal);
    public int TotalQuantity => Items.Sum(i => i.Quantity);
}

public class AddToCartDto
{
    [Required] public int ProductId { get; set; }
    [Range(1, 1000)] public int Quantity { get; set; } = 1;
}

public class UpdateCartItemDto
{
    [Range(0, 1000)] public int Quantity { get; set; }
}
