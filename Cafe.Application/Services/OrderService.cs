using Cafe.Application;
using Cafe.Application.Dtos;
using Cafe.Application.Interfaces;
using Cafe.Application.Models;
using Cafe.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Cafe.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<OrderService> logger) : IOrderService
    {
        public async Task<OrderResponse> CreateAsync(Guid userId, CreateOrderRequest request, CancellationToken ct = default)
        {
            // 1. Deduplica itens: duas linhas do mesmo produto viram uma
            var requestedItems = request.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) })
                .ToList();

            // 2. Busca os produtos UMA vez (Include evita N queries)
            var productIds = requestedItems.Select(i => i.ProductId).ToList();
            var products = await productRepository.GetByIdsAsync(productIds, ct);

            var missing = productIds.Except(products.Select(p => p.Id)).ToList();
            if (missing.Count > 0)
                throw new NotFoundException("Produto", string.Join(", ", missing));

            // 3. Valida estoque ANTES de montar (mensagem amigável)
            foreach (var item in requestedItems)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (!product.IsActive)
                    throw new ConflictException($"O produto '{product.Name}' não está mais disponível.");
                if (product.Stock < item.Quantity)
                    throw new ConflictException($"Estoque insuficiente para '{product.Name}'. Disponível: {product.Stock}, solicitado: {item.Quantity}.");
            }

            // 4. Monta o pedido — preço CONGELADO do servidor
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items = requestedItems.Select(item =>
                {
                    var product = products.First(p => p.Id == item.ProductId);
                    return new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price  // ← preço vem do servidor, nunca do cliente
                    };
                }).ToList()
            };
            order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            // 5. Decrementa estoque (a RowVersion protege a concorrência)
            await using var transaction = await orderRepository.BeginTransactionAsync(ct);

            foreach (var item in requestedItems)
            {
                var product = products.First(p => p.Id == item.ProductId);
                product.Stock -= item.Quantity;
            }

            orderRepository.Add(order);                    // marca o pedido (não salva)
            await orderRepository.SaveChangesAsync(ct);    // um save: pedido + itens + estoque

            await transaction.CommitAsync(ct);             

            logger.LogInformation("Pedido {OrderId} criado para o usuário {UserId}. Total: {Total}",
                order.Id, userId, order.Total);

            return MapToResponse(order);
        }

        public async Task<PagedResult<OrderResponse>> GetMyOrdersAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (items, totalCount) = await orderRepository.GetPagedByUserAsync(userId, page, pageSize, ct);
            return new PagedResult<OrderResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<OrderResponse>> GetAllAsync( int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (items, totalCount) = await orderRepository.GetPagedByUserAsync(null, page, pageSize, ct);
            return new PagedResult<OrderResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderResponse> GetByIdForUserAsync(Guid orderId, Guid userId, bool isAdmin, CancellationToken ct = default)
        {
            var order = await orderRepository.GetByIdAsync(orderId, ct)
                ?? throw new NotFoundException("Pedido", orderId);

            // dono do pedido ou admin — senão, para o cliente é como se não existisse
            if (order.UserId != userId && !isAdmin)
                throw new NotFoundException("Pedido", orderId);

            return MapToResponse(order);
        }

        private static OrderResponse MapToResponse(Order o) => new()
        {
            Id = o.Id,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            Total = o.Total,
            UserId = o.UserId,
            Items = o.Items.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }


}