using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs
{
    public class OrderSummaryDto
    {
        public OrderStatus Status { get; set; }
        public int Count { get; set; }
    }
}


