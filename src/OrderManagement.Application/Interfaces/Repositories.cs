using FlexiMarket.OrderManagement.Domain.Entities;
using FlexiMarket.OrderManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    Task<int> CountPendingOrdersForCustomerAsync(CustomerId customerId, CancellationToken cancellationToken);
}

public interface IOrderReadRepository
{
    Task<OrderDetailsDto> GetOrderDetailsAsync(Guid orderId, CancellationToken cancellationToken);
    Task<PagedResult<OrderSummaryDto>> GetOrdersAsync(OrderSearchCriteria criteria, CancellationToken cancellationToken);
}

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
