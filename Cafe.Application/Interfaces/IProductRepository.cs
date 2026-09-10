using Cafe.Domain.Entities;

namespace Cafe.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search, Guid? categoryId, CancellationToken ct = default);

        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task AddAsync(Product product, CancellationToken ct = default);

        Task UpdateAsync(Product product, CancellationToken ct = default);

        Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);

        Task<IReadOnlyList<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default);

        Task UpdateRangeAsync(CancellationToken ct = default);
    }
}
