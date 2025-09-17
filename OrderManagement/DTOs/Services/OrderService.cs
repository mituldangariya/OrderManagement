using Microsoft.Extensions.Logging;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.DTOs;
using OrderManagement.Infrastructure.Repositories;
using System.Globalization;

namespace OrderManagement.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _repo;
        private readonly ILogger<OrderService> _logger;

        public OrderService(OrderRepository repo, ILogger<OrderService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<OrderDetailsDto> CreateOrderAsync(CreateOrderDto dto)
        {
            _logger.LogInformation("Creating new order for customer {CustomerName}", dto.CustomerName);

            if (dto == null)
            {
                _logger.LogError("The order DTO is null.");
                throw new ArgumentNullException(nameof(dto), "Order data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                _logger.LogError("Customer name is required.");
                throw new ArgumentException("Customer name is required.", nameof(dto.CustomerName));
            }

            if (dto.Items == null || !dto.Items.Any())
            {
                _logger.LogError("At least one order item is required.");
                throw new ArgumentException("At least one order item is required.", nameof(dto.Items));
            }

            foreach (var item in dto.Items)
            {
                if (string.IsNullOrWhiteSpace(item.ItemName))
                {
                    _logger.LogError("Item name is required for all items.");
                    throw new ArgumentException("Item name is required.", nameof(item.ItemName));
                }

                if (item.Quantity <= 0)
                {
                    _logger.LogError("Item quantity must be greater than zero.");
                    throw new ArgumentException("Item quantity must be greater than zero.", nameof(item.Quantity));
                }

                if (item.UnitPrice <= 0)
                {
                    _logger.LogError("Item unit price must be greater than zero.");
                    throw new ArgumentException("Item unit price must be greater than zero.", nameof(item.UnitPrice));
                }
            }

            _logger.LogInformation("Creating new order for customer {CustomerName}", dto.CustomerName);


            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India S    tandard Time");
            var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = dto.CustomerName,
                Notes = dto.Notes,
                Status = OrderStatus.Pending,
                CreatedAt = indiaTime,   // ✅ IST
                Items = dto.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _repo.AddAsync(order);

            _logger.LogInformation("Order {OrderId} created successfully", order.Id);

            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                FromStatus = OrderStatus.Pending,
                ToStatus = OrderStatus.Pending,
                ChangedBy = dto.ChangedBy,
                ChangedAt = DateTime.UtcNow,
                Reason = dto.Reason
            };

            await _repo.AddHistoryAsync(history);

            _logger.LogInformation("Initial history for Order {OrderId} added by {ChangedBy}", order.Id, dto.ChangedBy);

            return MapToDto(order);
        }

        public async Task<OrderDetailsDto?> GetOrderByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching order {OrderId}", id);
            var order = await _repo.GetByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", id);
                return null;
            }

            return MapToDto(order);
        }

        public async Task<List<OrderDetailsDto>> ListOrdersAsync(string sortBy, bool desc)
        {
            //_logger.LogInformation("Fetching all orders");
            //var orders = await _repo.ListAsync();
            //return orders.Select(MapToDto).ToList();

            _logger.LogInformation("Fetching orders sorted by {SortBy}, desc = {Desc}", sortBy, desc);

            var orders = await _repo.ListAsync(); // returns IEnumerable<Order>

            // Normalize column name
            sortBy = (sortBy ?? "CreatedAt").Trim().ToLowerInvariant();

            IEnumerable<Order> sorted = sortBy switch
            {
                "customername" => desc
                    ? orders.OrderByDescending(o => o.CustomerName)
                    : orders.OrderBy(o => o.CustomerName),

                "updatedat" => desc
                    ? orders.OrderByDescending(o => o.UpdatedAt)
                    : orders.OrderBy(o => o.UpdatedAt),

                "createdat" => desc
                    ? orders.OrderByDescending(o => o.CreatedAt)
                    : orders.OrderBy(o => o.CreatedAt),

                _ => desc
                    ? orders.OrderByDescending(o => o.CreatedAt) // default fallback
                    : orders.OrderBy(o => o.CreatedAt)
            };

            return sorted.Select(MapToDto).ToList();
        }

        public async Task<OrderDetailsDto?> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto)
        {
            _logger.LogInformation("Updating status for Order {OrderId} to {NewStatus}", id, dto.NewStatus);

            var order = await _repo.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for status update", id);
                return null;
            }

            var oldStatus = order.Status;
            order.Status = dto.NewStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(order);
            _logger.LogInformation("Order {OrderId} status updated from {OldStatus} to {NewStatus}", id, oldStatus, dto.NewStatus);

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
            _logger.LogInformation("History entry for Order {OrderId} added by {ChangedBy}", order.Id, dto.ChangedBy);

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

        public async Task<List<OrderSummaryDto>> GetSummaryAsync(OrderSummaryRequestDto request)
        {
            _logger.LogInformation("Fetching order summary from {From} to {To}", request.FromDate, request.ToDate);
            return await _repo.GetOrderSummaryAsync(request.FromDate, request.ToDate);
        }

    }
}
