using FlexiMarket.OrderManagement.Domain.Entities;
using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    Task<int> CountPendingOrdersForCustomerAsync(CustomerId customerId, CancellationToken cancellationToken);
}
