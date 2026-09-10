using Cafe.Application.Dtos;
using Cafe.Application.Interfaces;
using Cafe.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductService service): ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        CancellationToken ct = default)
        {
            var result = await service.GetPagedAsync(page, pageSize, search, categoryId, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken ct)
        {
            var product = await service.GetByIdAsync(id, ct);
            return product is null ? NotFound() : Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request, CancellationToken ct)
        {
            var created = await service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest request, CancellationToken ct)
        {
            var updated = await service.UpdateAsync(id, request, ct);
            return Ok(updated);
        }
    }
}
