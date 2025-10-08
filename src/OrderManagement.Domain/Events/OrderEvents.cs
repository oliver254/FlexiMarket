
using FlexiMarket.SharedKernel;

namespace FlexiMarket.OrderManagement.Domain.Events;


public record OrderCreatedEvent(Guid OrderId, Guid CustomerId) : DomainEvent;
public record OrderSubmittedEvent(Guid OrderId, decimal TotalAmount) : DomainEvent;
public record OrderConfirmedEvent(Guid OrderId) : DomainEvent;
public record OrderProcessingStartedEvent(Guid OrderId) : DomainEvent;
public record OrderShippedEvent(Guid OrderId, string TrackingNumber) : DomainEvent;
public record OrderCompletedEvent(Guid OrderId, DateTime CompletedAt) : DomainEvent;
public record OrderCancelledEvent(Guid OrderId, string Reason) : DomainEvent;
public record ProductAddedToOrderEvent(Guid OrderId, Guid ProductId, int Quantity) : DomainEvent;
public record ProductRemovedFromOrderEvent(Guid OrderId, Guid ProductId) : DomainEvent;