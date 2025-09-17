using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories
{
    public class OrderRepository
    {
        private readonly OrdersDbContext _context;
        public OrderRepository(OrdersDbContext context) => _context = context;

        public async Task<Order?> GetByIdAsync(Guid id) =>
            await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Histories)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<Order>> ListAsync() =>
            await _context.Orders.Include(o => o.Items).ToListAsync();

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddHistoryAsync(OrderHistory history)
        {
            await _context.OrderHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderSummaryDto>> GetOrderSummaryAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt <= toDate.Value);

            return await query
                .GroupBy(o => o.Status)
                .Select(g => new OrderSummaryDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }
    }
}
