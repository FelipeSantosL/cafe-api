using Cafe.Application.Interfaces;
using Cafe.Domain.Entities;
using Cafe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Infrastructure.Repositories
{
    public class OrderRepository(CafeDbContext db) : IOrderRepository
    {
        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedByUserAsync(
            Guid? userId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = db.Orders.AsQueryable();
            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        // Só marca no change tracker — quem salva é o SaveChangesAsync
        public void Add(Order order)
            => db.Orders.Add(order);

        public Task SaveChangesAsync(CancellationToken ct = default)
            => db.SaveChangesAsync(ct);

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default) => await db.Database.BeginTransactionAsync(ct);
    }
}