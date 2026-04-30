using MyStore.Api.Models.Dtos;
using MyStore.Api.Models.Entities;

namespace MyStore.Api.Services;

public interface IOrderService
{
    Task<CheckoutResponse> CheckoutAsync(int userId, CreateOrderDto dto);
    Task<List<OrderDto>> ListForUserAsync(int userId);
    Task<OrderDto?> GetForUserAsync(int userId, int orderId);
    Task<List<OrderDto>> ListAllAsync();
    Task<OrderDto?> UpdateStatusAsync(int orderId, OrderStatus status);
}
