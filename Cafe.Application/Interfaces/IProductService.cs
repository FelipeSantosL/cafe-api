using Cafe.Application.Dtos;
using Cafe.Application.Models;


namespace Cafe.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponse>> GetPagedAsync(int page, int pageSize, string? search, Guid? categoryId, CancellationToken ct = default);
        Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
        Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    }
}
