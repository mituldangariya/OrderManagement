using OrderManagement.Application.DTOs;
using OrderManagement.DTOs;

namespace OrderManagement.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDetailsDto> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDetailsDto?> GetOrderByIdAsync(Guid id);
        Task<List<OrderDetailsDto>> ListOrdersAsync(string sortBy, bool desc);
        Task<OrderDetailsDto?> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto);
        Task<List<OrderSummaryDto>> GetSummaryAsync(OrderSummaryRequestDto request);

    }
}
