using MyStore.Api.Models.Dtos;
using MyStore.Api.Models.Entities;
using MyStore.Api.Repositories;

namespace MyStore.Api.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cart;
    private readonly IProductRepository _products;

    public CartService(ICartRepository cart, IProductRepository products)
    {
        _cart = cart;
        _products = products;
    }

    public async Task<CartDto> GetAsync(int userId)
    {
        var items = await _cart.ListByUserAsync(userId);
        return new CartDto { Items = items.Select(ToDto).ToList() };
    }

    public async Task<CartDto> AddAsync(int userId, AddToCartDto dto)
    {
        var product = await _products.GetByIdAsync(dto.ProductId)
            ?? throw new InvalidOperationException("Product not found.");
        if (product.Stock < dto.Quantity)
            throw new InvalidOperationException("Insufficient stock.");

        var existing = await _cart.GetByUserAndProductAsync(userId, dto.ProductId);
        if (existing is null)
        {
            await _cart.AddAsync(new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Quantity += dto.Quantity;
            if (existing.Quantity > product.Stock) existing.Quantity = product.Stock;
            existing.UpdatedAt = DateTime.UtcNow;
            _cart.Update(existing);
        }
        await _cart.SaveChangesAsync();
        return await GetAsync(userId);
    }

    public async Task<CartDto> UpdateAsync(int userId, int productId, UpdateCartItemDto dto)
    {
        var item = await _cart.GetByUserAndProductAsync(userId, productId);
        if (item is null) return await GetAsync(userId);

        if (dto.Quantity <= 0)
        {
            _cart.Remove(item);
        }
        else
        {
            var product = await _products.GetByIdAsync(productId);
            if (product is null) throw new InvalidOperationException("Product not found.");
            item.Quantity = Math.Min(dto.Quantity, product.Stock);
            item.UpdatedAt = DateTime.UtcNow;
            _cart.Update(item);
        }
        await _cart.SaveChangesAsync();
        return await GetAsync(userId);
    }

    public async Task<CartDto> RemoveAsync(int userId, int productId)
    {
        var item = await _cart.GetByUserAndProductAsync(userId, productId);
        if (item is not null)
        {
            _cart.Remove(item);
            await _cart.SaveChangesAsync();
        }
        return await GetAsync(userId);
    }

    public async Task<CartDto> ClearAsync(int userId)
    {
        await _cart.ClearAsync(userId);
        await _cart.SaveChangesAsync();
        return new CartDto();
    }

    private static CartItemDto ToDto(CartItem i) => new()
    {
        ProductId = i.ProductId,
        ProductName = i.Product?.Name ?? string.Empty,
        ImageUrl = i.Product?.ImageUrl ?? string.Empty,
        UnitPrice = i.Product?.Price ?? 0,
        Quantity = i.Quantity,
        Stock = i.Product?.Stock ?? 0
    };
}
