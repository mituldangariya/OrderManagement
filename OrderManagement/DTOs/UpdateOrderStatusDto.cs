using System.ComponentModel.DataAnnotations;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required] public OrderStatus NewStatus { get; set; }
        [Required] public string ChangedBy { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}
