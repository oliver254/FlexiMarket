using FlexiMarket.OrderManagement.Domain.Events;
using FlexiMarket.OrderManagement.Domain.ValueObjects;
using FlexiMarket.SharedKernel;
using FlexiMarket.SharedKernel.Interfaces;

namespace FlexiMarket.OrderManagement.Domain.Entities;

public class Order : AggregateRoot
{
    private readonly List<OrderLine> _orderLines = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public Address ShippingAddress { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Order() { } // Pour EF Core

    public static Order Create(CustomerId customerId, Address shippingAddress)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            ShippingAddress = shippingAddress,
            Status = OrderStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = Money.Zero("EUR")
        };

        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerId.Value));
        return order;
    }

    public void AddProduct(ProductId productId, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify a non-draft order");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var existingLine = _orderLines.FirstOrDefault(l => l.ProductId == productId);

        if (existingLine != null)
        {
            existingLine.UpdateQuantity(existingLine.Quantity + quantity);
        }
        else
        {
            var line = OrderLine.Create(productId, quantity, unitPrice);
            _orderLines.Add(line);
        }

        RecalculateTotalAmount();
        AddDomainEvent(new ProductAddedToOrderEvent(Id, productId.Value, quantity));
    }

    public void RemoveProduct(ProductId productId)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify a non-draft order");

        var line = _orderLines.FirstOrDefault(l => l.ProductId == productId);
        if (line == null)
            throw new InvalidOperationException("Product not found in order");

        _orderLines.Remove(line);
        RecalculateTotalAmount();
        AddDomainEvent(new ProductRemovedFromOrderEvent(Id, productId.Value));
    }

    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be submitted");

        if (!_orderLines.Any())
            throw new InvalidOperationException("Cannot submit empty order");

        Status = OrderStatus.Pending;
        AddDomainEvent(new OrderSubmittedEvent(Id, TotalAmount.Amount));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Id));
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be processed");

        Status = OrderStatus.Processing;
        AddDomainEvent(new OrderProcessingStartedEvent(Id));
    }

    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be shipped");

        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number is required", nameof(trackingNumber));

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(Id, trackingNumber));
    }

    public void Complete()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be completed");

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderCompletedEvent(Id, CompletedAt.Value));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel completed or already cancelled order");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id, reason));
    }

    private void RecalculateTotalAmount()
    {
        var total = _orderLines.Sum(l => l.LineTotal.Amount);
        TotalAmount = new Money(total, "EUR");
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
}
