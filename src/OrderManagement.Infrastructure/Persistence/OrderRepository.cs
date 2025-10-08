using FlexiMarket.OrderManagement.Application.Interfaces;
using FlexiMarket.OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiMarket.OrderManagement.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        _context.Orders.Update(order);
    }

    public async Task<int> CountPendingOrdersForCustomerAsync(CustomerId customerId, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Where(o => o.CustomerId == customerId && o.Status == OrderStatus.Pending)
            .CountAsync(cancellationToken);
    }
}

