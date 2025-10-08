using FlexiMarket.OrderManagement.Application.Interfaces;
using FlexiMarket.OrderManagement.Domain.Entities;
using FlexiMarket.OrderManagement.Domain.Exceptions;
using FlexiMarket.OrderManagement.Domain.ValueObjects;
using FlexiMarket.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlexiMarket.OrderManagement.Application.UseCases.Commands;

public record CreateOrderCommand : ICommand<CreateOrderResult>
{
    public Guid CustomerId { get; init; }
    public AddressDto ShippingAddress { get; init; }
}

public record CreateOrderResult(Guid OrderId, string OrderNumber);

public record AddressDto(string Street, string City, string PostalCode, string Country);

// Handler avec validation métier complexe
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // Validation métier : le client existe et est actif
        var customer = await _customerRepository.GetByIdAsync(new CustomerId(command.CustomerId), cancellationToken);
        if (customer == null)
            throw new BusinessException("Customer not found");

        if (!customer.IsActive)
            throw new BusinessException("Customer account is inactive");

        // Vérification de la limite de commandes en cours
        var pendingOrdersCount = await _orderRepository.CountPendingOrdersForCustomerAsync(
            new CustomerId(command.CustomerId),
            cancellationToken);

        if (pendingOrdersCount >= 5)
            throw new BusinessException("Customer has reached the maximum number of pending orders");

        // Création de la commande
        var shippingAddress = new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.PostalCode,
            command.ShippingAddress.Country);

        var order = Order.Create(new CustomerId(command.CustomerId), shippingAddress);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Order {OrderNumber} created for customer {CustomerId}",
            order.OrderNumber, command.CustomerId);

        return new CreateOrderResult(order.Id, order.OrderNumber);
    }
}
