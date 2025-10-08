using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Domain.Entities;

public class Customer
{
    public CustomerId Id { get; set; }
    public bool IsActive { get; set; }

    public bool HasOutstandingDebts()
    {
        // Logique métier
        return false;
    }
}
