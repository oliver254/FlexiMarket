using FlexiMarket.OrderManagement.Domain.Entities;
using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken);
}
