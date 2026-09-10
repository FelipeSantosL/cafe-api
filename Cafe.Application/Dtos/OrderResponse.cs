using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = null!;
        public decimal Total { get; set; }
        public Guid UserId { get; set; }
        public List<OrderItemResponse> Items { get; set; } = [];
    }

    public class OrderItemResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}