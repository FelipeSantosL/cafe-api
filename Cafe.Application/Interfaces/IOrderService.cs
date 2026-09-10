using Cafe.Application.Dtos;
using Cafe.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateAsync(Guid userId, CreateOrderRequest request, CancellationToken ct = default);
        Task<PagedResult<OrderResponse>> GetMyOrdersAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
        Task<PagedResult<OrderResponse>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        Task<OrderResponse> GetByIdForUserAsync(Guid orderId, Guid userId, bool isAdmin, CancellationToken ct = default);
    }
}