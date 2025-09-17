using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Repositories;

namespace OrderManagement.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _repo;
        public OrderService(OrderRepository repo) => _repo = repo;

        public async Task<OrderDetailsDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = dto.CustomerName,
                Notes = dto.Notes,
                Items = dto.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _repo.AddAsync(order);
            return MapToDto(order);
        }

        public async Task<OrderDetailsDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order == null ? null : MapToDto(order);
        }

        public async Task<List<OrderDetailsDto>> ListOrdersAsync()
        {
            var orders = await _repo.ListAsync();
            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderDetailsDto?> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return null;

            var oldStatus = order.Status;
            order.Status = dto.NewStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(order);

            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                FromStatus = oldStatus,
                ToStatus = dto.NewStatus,
                ChangedBy = dto.ChangedBy,
                Reason = dto.Reason,
                ChangedAt = DateTime.UtcNow
            };
            await _repo.AddHistoryAsync(history);

            return MapToDto(order);
        }

        private static OrderDetailsDto MapToDto(Order order) => new()
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Notes = order.Notes,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            Histories = order.Histories.Select(h => new OrderHistoryDto
            {
                FromStatus = h.FromStatus,
                ToStatus = h.ToStatus,
                ChangedBy = h.ChangedBy,
                ChangedAt = h.ChangedAt,
                Reason = h.Reason
            }).ToList()
        };
    }
}
