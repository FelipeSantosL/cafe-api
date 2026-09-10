using Cafe.Application.Dtos;
using Cafe.Application.Interfaces;
using Cafe.Application.Models;
using Cafe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Services
{
    public class ProductService(IProductRepository repo) : IProductService
    {
        public async Task<PagedResult<ProductResponse>> GetPagedAsync(int page, int pageSize, string? search, Guid? categoryId, CancellationToken ct = default)
        {
            // Proteção contra parâmetros inválidos vindos da query string
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (items, totalCount) = await repo.GetPagedAsync(page, pageSize, search, categoryId, ct);

            return new PagedResult<ProductResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var product = await repo.GetByIdAsync(id, ct);
            return product is null ? null : MapToResponse(product);
        }

        private static ProductResponse MapToResponse(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name
        };
        public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
        {
            // Regra de negócio: nome de produto é único
            if (await repo.NameExistsAsync(request.Name, ct: ct))
                throw new ConflictException($"Já existe um produto com o nome '{request.Name}'.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                IsActive = true,
                CategoryId = request.CategoryId
            };

            await repo.AddAsync(product, ct);

            var created = await repo.GetByIdAsync(product.Id, ct);
            return MapToResponse(created!);
        }
        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
        {
            var product = await repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException("Produto", id);

            if (await repo.NameExistsAsync(request.Name, excludeId: id, ct: ct))
                throw new ConflictException($"Já existe outro produto com o nome '{request.Name}'.");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.IsActive = request.IsActive;
            product.CategoryId = request.CategoryId;

            await repo.UpdateAsync(product, ct);

            var updated = await repo.GetByIdAsync(id, ct);
            return MapToResponse(updated!);
        }
    }
}