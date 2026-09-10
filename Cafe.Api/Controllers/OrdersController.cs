using System.Security.Claims;
using Cafe.Application.Dtos;
using Cafe.Application.Interfaces;
using Cafe.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class OrdersController(IOrderService service) : ControllerBase
    {

        private Guid UserId =>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

        private bool IsAdmin => User.IsInRole("Admin");

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken ct)
        {
            var order = await service.CreateAsync(UserId, request, ct);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpGet("mine")]
        public async Task<ActionResult<PagedResult<OrderResponse>>> MyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) => Ok(await service.GetMyOrdersAsync(UserId, page, pageSize, ct));

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<OrderResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)  => Ok(await service.GetAllAsync(page, pageSize, ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken ct) => Ok(await service.GetByIdForUserAsync(id, UserId, IsAdmin, ct));
    }
}

