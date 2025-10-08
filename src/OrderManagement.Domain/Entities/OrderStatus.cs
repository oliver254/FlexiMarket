namespace FlexiMarket.OrderManagement.Domain.Entities;

public enum OrderStatus
{
    Draft,
    Pending,
    Confirmed,
    Processing,
    Shipped,
    Completed,
    Cancelled
}
