using System;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities
{
    public class OrderHistory
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderStatus FromStatus { get; set; }
        public OrderStatus ToStatus { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }

        public Order? Order { get; set; }
    }
}
