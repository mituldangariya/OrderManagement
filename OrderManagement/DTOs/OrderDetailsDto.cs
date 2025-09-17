using System;
using System.Collections.Generic;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs
{
    public class OrderDetailsDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public List<OrderHistoryDto> Histories { get; set; } = new();
    }

    public class OrderItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderHistoryDto
    {
        public OrderStatus FromStatus { get; set; }
        public OrderStatus ToStatus { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? Reason { get; set; }
    }
}
