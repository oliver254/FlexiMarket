using FlexiMarket.OrderManagement.Application.DTOs;
using FlexiMarket.OrderManagement.Application.Interfaces;
using FlexiMarket.OrderManagement.Domain.Entities;
using FlexiMarket.OrderManagement.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FlexiMarket.OrderManagement.Application.Services;

public class OrderSubmissionService : IOrderSubmissionService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IPricingService _pricingService;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderSubmissionService> _logger;

    public OrderSubmissionService(
        IOrderRepository orderRepository,
        IInventoryService inventoryService,
        IPricingService pricingService,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        ILogger<OrderSubmissionService> logger)
    {
        _orderRepository = orderRepository;
        _inventoryService = inventoryService;
        _pricingService = pricingService;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<OrderSubmissionResult> SubmitOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting order submission workflow for {OrderId}", orderId);

        var context = new SubmissionContext();

        try
        {
            // 1️⃣ Récupérer et valider la commande
            var order = await GetAndValidateOrderAsync(orderId, cancellationToken);

            // 2️⃣ Vérifier la disponibilité des produits
            await ValidateProductAvailabilityAsync(order, cancellationToken);

            // 3️⃣ Valider les prix actuels
            await ValidatePricesAsync(order, cancellationToken);

            // 4️⃣ Pré-autoriser le paiement
            context.PaymentAuthId = await PreAuthorizePaymentAsync(order, cancellationToken);

            // 5️⃣ Réserver l'inventaire
            context.ReservationId = await ReserveInventoryAsync(order, cancellationToken);

            // 6️⃣ Soumettre la commande dans le domaine
            await SubmitOrderInDomainAsync(order, cancellationToken);

            // 7️⃣ Publier les événements de domaine
            await PublishDomainEventsAsync(order, cancellationToken);

            _logger.LogInformation(
                "Order {OrderId} submitted successfully. Reservation: {ReservationId}, Payment: {PaymentAuthId}",
                orderId, context.ReservationId, context.PaymentAuthId);

            return OrderSubmissionResult.SuccessResult(context.ReservationId, context.PaymentAuthId);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business validation failed for order {OrderId}", orderId);
            await CompensateAsync(context, cancellationToken);
            return OrderSubmissionResult.Failed(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Order submission failed for {OrderId}", orderId);
            await CompensateAsync(context, cancellationToken);
            throw;
        }
    }

    private async Task<Order> GetAndValidateOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order == null)
            throw new NotFoundException("Order not found");

        if (order.Status != OrderStatus.Draft)
            throw new BusinessException("Only draft orders can be submitted");

        if (!order.OrderLines.Any())
            throw new BusinessException("Cannot submit empty order");

        return order;
    }

    private async Task ValidateProductAvailabilityAsync(Order order, CancellationToken cancellationToken)
    {
        foreach (var line in order.OrderLines)
        {
            var isAvailable = await _inventoryService.CheckAvailabilityAsync(
                line.ProductId,
                line.Quantity,
                cancellationToken);

            if (!isAvailable)
            {
                _logger.LogWarning("Product {ProductId} not available in requested quantity", line.ProductId);
                throw new BusinessException($"Product {line.ProductId} is not available in requested quantity");
            }
        }
    }

    private async Task ValidatePricesAsync(Order order, CancellationToken cancellationToken)
    {
        foreach (var line in order.OrderLines)
        {
            var currentPrice = await _pricingService.GetCurrentPriceAsync(line.ProductId, cancellationToken);

            if (currentPrice.Amount != line.UnitPrice.Amount)
            {
                _logger.LogWarning(
                    "Price changed for product {ProductId}. Old: {OldPrice}, New: {NewPrice}",
                    line.ProductId, line.UnitPrice, currentPrice);

                throw new BusinessException(
                    "Some product prices have changed. Please review your order.");
            }
        }
    }

    private async Task<string> PreAuthorizePaymentAsync(Order order, CancellationToken cancellationToken)
    {
        var preAuthResult = await _paymentService.PreAuthorizeAsync(
            order.CustomerId,
            order.TotalAmount,
            cancellationToken);

        if (!preAuthResult.Success)
        {
            _logger.LogWarning("Payment pre-authorization failed for order {OrderId}", order.Id);
            throw new BusinessException("Payment authorization failed");
        }

        return preAuthResult.AuthorizationId;
    }

    private async Task<string> ReserveInventoryAsync(Order order, CancellationToken cancellationToken)
    {
        var reservationId = await _inventoryService.ReserveInventoryAsync(
            order.Id,
            order.OrderLines.Select(l => new InventoryReservationItem(l.ProductId, l.Quantity)).ToList(),
            cancellationToken);

        return reservationId;
    }

    private async Task SubmitOrderInDomainAsync(Order order, CancellationToken cancellationToken)
    {
        order.Submit();
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(Order order, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            await _eventBus.PublishAsync(domainEvent, cancellationToken);
        }
        order.ClearDomainEvents();
    }

    private async Task CompensateAsync(SubmissionContext context, CancellationToken cancellationToken)
    {
        // Compensation dans l'ordre inverse
        if (!string.IsNullOrEmpty(context.ReservationId))
        {
            try
            {
                _logger.LogInformation("Cancelling inventory reservation {ReservationId}", context.ReservationId);
                await _inventoryService.CancelReservationAsync(context.ReservationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel inventory reservation {ReservationId}", context.ReservationId);
            }
        }

        if (!string.IsNullOrEmpty(context.PaymentAuthId))
        {
            try
            {
                _logger.LogInformation("Cancelling payment authorization {PaymentAuthId}", context.PaymentAuthId);
                await _paymentService.CancelPreAuthorizationAsync(context.PaymentAuthId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel payment authorization {PaymentAuthId}", context.PaymentAuthId);
            }
        }
    }

    private class SubmissionContext
    {
        public string ReservationId { get; set; }
        public string PaymentAuthId { get; set; }
    }
}

