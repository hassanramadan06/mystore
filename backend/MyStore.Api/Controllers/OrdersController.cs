using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Api.Models.Dtos;
using MyStore.Api.Models.Entities;
using MyStore.Api.Services;

namespace MyStore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders) => _orders = orders;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("checkout")]
    public Task<CheckoutResponse> Checkout([FromBody] CreateOrderDto dto) =>
        _orders.CheckoutAsync(UserId, dto);

    [HttpGet]
    public Task<List<OrderDto>> Mine() => _orders.ListForUserAsync(UserId);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> Get(int id)
    {
        var order = await _orders.GetForUserAsync(UserId, id);
        return order is null ? NotFound() : Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public Task<List<OrderDto>> All() => _orders.ListAllAsync();

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(int id, [FromBody] OrderStatus status)
    {
        var updated = await _orders.UpdateStatusAsync(id, status);
        return updated is null ? NotFound() : Ok(updated);
    }
}
