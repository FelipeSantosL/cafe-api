using Cafe.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedByUserAsync(Guid? userId, int page, int pageSize, CancellationToken ct = default);

        void Add(Order order);

        Task SaveChangesAsync(CancellationToken ct = default);

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    }
}
