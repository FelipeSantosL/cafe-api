using Cafe.Application.Interfaces;
using Cafe.Domain.Entities;
using Cafe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Cafe.Infrastructure.Repositories
{
    public class ProductRepository(CafeDbContext db) : IProductRepository
    {
        public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search, Guid? categoryId, CancellationToken ct = default)
        {
            var query = db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task AddAsync(Product product, CancellationToken ct = default)
        {
            db.Products.Add(product);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Product product, CancellationToken ct = default)
        {
            db.Products.Update(product);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
        {
            return await db.Products
                .AnyAsync(p => p.Name == name && (excludeId == null || p.Id != excludeId), ct);
        }

        public async Task<IReadOnlyList<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default)
        {
            return await db.Products.Where(p => ids.Contains(p.Id)).ToListAsync(ct);
        }

        public Task UpdateRangeAsync(CancellationToken ct = default)
        {
            return db.SaveChangesAsync(ct);
        }
    }
}