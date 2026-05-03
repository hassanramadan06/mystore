using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Api.Models.Dtos;
using MyStore.Api.Services;

namespace MyStore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;

    public CartController(ICartService cart) => _cart = cart;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public Task<CartDto> Get() => _cart.GetAsync(UserId);

    [HttpPost("items")]
    public Task<CartDto> Add([FromBody] AddToCartDto dto) => _cart.AddAsync(UserId, dto);

    [HttpPut("items/{productId:int}")]
    public Task<CartDto> Update(int productId, [FromBody] UpdateCartItemDto dto) =>
        _cart.UpdateAsync(UserId, productId, dto);

    [HttpDelete("items/{productId:int}")]
    public Task<CartDto> Remove(int productId) => _cart.RemoveAsync(UserId, productId);

    [HttpDelete]
    public Task<CartDto> Clear() => _cart.ClearAsync(UserId);
}
