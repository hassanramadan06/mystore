using Microsoft.EntityFrameworkCore;
using MyStore.Api.Models.Dtos;
using MyStore.Api.Models.Entities;
using MyStore.Api.Repositories;

namespace MyStore.Api.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;
    private readonly ICartRepository _cart;
    private readonly IPaymentService _payments;

    public OrderService(
        IOrderRepository orders,
        IProductRepository products,
        ICartRepository cart,
        IPaymentService payments)
    {
        _orders = orders;
        _products = products;
        _cart = cart;
        _payments = payments;
    }

    public async Task<CheckoutResponse> CheckoutAsync(int userId, CreateOrderDto dto)
    {
        var inputs = dto.Items;
        if (inputs.Count == 0)
        {
            var cartItems = await _cart.ListByUserAsync(userId);
            inputs = cartItems
                .Select(c => new OrderItemInput { ProductId = c.ProductId, Quantity = c.Quantity })
                .ToList();
        }

        if (inputs.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        var productIds = inputs.Select(i => i.ProductId).Distinct().ToList();
        var products = await _products.Query()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0m;
        foreach (var input in inputs)
        {
            var product = products.FirstOrDefault(p => p.Id == input.ProductId)
                ?? throw new InvalidOperationException($"Product {input.ProductId} not found.");
            if (product.Stock < input.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}.");

            product.Stock -= input.Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = input.Quantity,
                UnitPrice = product.Price
            });
            total += product.Price * input.Quantity;
        }
        order.Total = total;

        await _orders.AddAsync(order);
        await _orders.SaveChangesAsync();

        var intent = await _payments.CreateIntentAsync(total, "usd", order.Id, userId);
        order.PaymentIntentId = intent.Id;
        _orders.Update(order);

        // Clear cart on successful order creation.
        await _cart.ClearAsync(userId);
        await _orders.SaveChangesAsync();

        var saved = await _orders.GetWithItemsAsync(order.Id);
        return new CheckoutResponse
        {
            Order = ToDto(saved!),
            ClientSecret = intent.ClientSecret,
            PublishableKey = _payments.PublishableKey
        };
    }

    public async Task<List<OrderDto>> ListForUserAsync(int userId)
    {
        var list = await _orders.ListByUserAsync(userId);
        return list.Select(ToDto).ToList();
    }

    public async Task<OrderDto?> GetForUserAsync(int userId, int orderId)
    {
        var order = await _orders.GetWithItemsAsync(orderId);
        if (order is null || order.UserId != userId) return null;
        return ToDto(order);
    }

    public async Task<List<OrderDto>> ListAllAsync()
    {
        var list = await _orders.QueryWithItems()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return list.Select(ToDto).ToList();
    }

    public async Task<OrderDto?> UpdateStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _orders.GetWithItemsAsync(orderId);
        if (order is null) return null;
        order.Status = status;
        _orders.Update(order);
        await _orders.SaveChangesAsync();
        return ToDto(order);
    }

    private static OrderDto ToDto(Order o) => new()
    {
        Id = o.Id,
        UserId = o.UserId,
        Total = o.Total,
        Status = o.Status,
        ShippingAddress = o.ShippingAddress,
        City = o.City,
        PostalCode = o.PostalCode,
        Country = o.Country,
        PaymentIntentId = o.PaymentIntentId,
        CreatedAt = o.CreatedAt,
        Items = o.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? string.Empty,
            ProductImageUrl = i.Product?.ImageUrl ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };
}
