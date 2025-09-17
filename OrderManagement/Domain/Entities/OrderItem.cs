using System;

namespace OrderManagement.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }= 0;
        public decimal UnitPrice { get; set; }

        public Order? Order { get; set; }
    }
}
