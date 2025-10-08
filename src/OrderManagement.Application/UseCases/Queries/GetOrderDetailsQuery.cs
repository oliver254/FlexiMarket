using FlexiMarket.OrderManagement.Application.Interfaces;
using FlexiMarket.OrderManagement.Application.UseCases.Commands;
using FlexiMarket.OrderManagement.Domain.Exceptions;
using FlexiMarket.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlexiMarket.OrderManagement.Application.UseCases.Queries;

public record GetOrderDetailsQuery(Guid OrderId) : IQuery<OrderDetailsDto>;

public record OrderDetailsDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; }
    public AddressDto ShippingAddress { get; init; }
    public string Status { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public List<OrderLineDto> OrderLines { get; init; }
}

public record OrderLineDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);

// Query handler - lecture optimisée
public class GetOrderDetailsQueryHandler : IQueryHandler<GetOrderDetailsQuery, OrderDetailsDto>
{
    private readonly IOrderReadRepository _orderReadRepository;
    private readonly ILogger<GetOrderDetailsQueryHandler> _logger;

    public GetOrderDetailsQueryHandler(
        IOrderReadRepository orderReadRepository,
        ILogger<GetOrderDetailsQueryHandler> logger)
    {
        _orderReadRepository = orderReadRepository;
        _logger = logger;
    }

    public async Task<OrderDetailsDto> Handle(GetOrderDetailsQuery query, CancellationToken cancellationToken)
    {
        var order = await _orderReadRepository.GetOrderDetailsAsync(query.OrderId, cancellationToken);

        if (order == null)
            throw new NotFoundException($"Order {query.OrderId} not found");

        return order;
    }
}
