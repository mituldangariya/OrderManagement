using System;
using System.Collections.Generic;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities
{
        public class Order
        {
            public Guid Id { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string? Notes { get; set; }
            public OrderStatus Status { get; set; } = OrderStatus.Pending;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }

            public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
            public ICollection<OrderHistory> Histories { get; set; } = new List<OrderHistory>();
        }
}
