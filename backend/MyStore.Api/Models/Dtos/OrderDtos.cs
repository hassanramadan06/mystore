using System.ComponentModel.DataAnnotations;
using MyStore.Api.Models.Entities;

namespace MyStore.Api.Models.Dtos;

public class CreateOrderDto
{
    [Required, MaxLength(300)] public string ShippingAddress { get; set; } = string.Empty;
    [Required, MaxLength(60)]  public string City { get; set; } = string.Empty;
    [Required, MaxLength(20)]  public string PostalCode { get; set; } = string.Empty;
    [Required, MaxLength(60)]  public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Optional explicit items. If empty, the user's current cart is used.
    /// </summary>
    public List<OrderItemInput> Items { get; set; } = new();
}

public class OrderItemInput
{
    [Required] public int ProductId { get; set; }
    [Range(1, 1000)] public int Quantity { get; set; } = 1;
}

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? PaymentIntentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

public class CheckoutResponse
{
    public OrderDto Order { get; set; } = new();
    public string ClientSecret { get; set; } = string.Empty; // Stripe client_secret (stub)
    public string PublishableKey { get; set; } = string.Empty;
}
