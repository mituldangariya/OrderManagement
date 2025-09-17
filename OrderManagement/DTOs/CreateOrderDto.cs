using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs
{
    public class CreateOrderDto
    {
        [Required] public string CustomerName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();

        // Add fields for history
        public string ChangedBy { get; set; } = "system"; // default
        public string Reason { get; set; } = "Order created"; // default
    }

    public class CreateOrderItemDto
    {
        [Required] public string ItemName { get; set; } = string.Empty;
        [Range(1, int.MaxValue)] public int Quantity { get; set; } = 0;
        [Range(0, double.MaxValue)] public decimal UnitPrice { get; set; }
    }
}
