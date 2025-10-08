using FlexiMarket.OrderManagement.Application.Interfaces;
using FlexiMarket.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiMarket.OrderManagement.Application.UseCases.Commands;

public record SubmitOrderCommand(Guid OrderId) : ICommand<SubmitOrderResult>;

public record SubmitOrderResult(bool Success, string Message);

// Use case métier complexe : soumission avec validations multiples
public class SubmitOrderCommandHandler : ICommandHandler<SubmitOrderCommand, SubmitOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IPricingService _pricingService;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;
    private readonly ILogger<SubmitOrderCommandHandler> _logger;

    public SubmitOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IInventoryService inventoryService,
        IPricingService pricingService,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        ILogger<SubmitOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _inventoryService = inventoryService;
        _pricingService = pricingService;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<SubmitOrderResult> Handle(SubmitOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order == null)
            throw new NotFoundException("Order not found");

        // 1. Vérification de la disponibilité des produits
        foreach (var line in order.OrderLines)
        {
            var isAvailable = await _inventoryService.CheckAvailabilityAsync(
                line.ProductId,
                line.Quantity,
                cancellationToken);

            if (!isAvailable)
            {
                _logger.LogWarning("Product {ProductId} not available in requested quantity", line.ProductId);
                return new SubmitOrderResult(false, $"Product {line.ProductId} is not available");
            }
        }

        // 2. Validation des prix (détection de modifications depuis l'ajout)
        foreach (var line in order.OrderLines)
        {
            var currentPrice = await _pricingService.GetCurrentPriceAsync(line.ProductId, cancellationToken);

            if (currentPrice.Amount != line.UnitPrice.Amount)
            {
                _logger.LogWarning("Price changed for product {ProductId}. Old: {OldPrice}, New: {NewPrice}",
                    line.ProductId, line.UnitPrice, currentPrice);

                return new SubmitOrderResult(false,
                    "Some product prices have changed. Please review your order.");
            }
        }

        // 3. Pré-autorisation du paiement
        var preAuthResult = await _paymentService.PreAuthorizeAsync(
            order.CustomerId,
            order.TotalAmount,
            cancellationToken);

        if (!preAuthResult.Success)
        {
            _logger.LogWarning("Payment pre-authorization failed for order {OrderId}", order.Id);
            return new SubmitOrderResult(false, "Payment authorization failed");
        }

        // 4. Réservation de l'inventaire
        var reservationId = await _inventoryService.ReserveInventoryAsync(
            order.Id,
            order.OrderLines.Select(l => new InventoryReservationItem(l.ProductId, l.Quantity)).ToList(),
            cancellationToken);

        // 5. Soumission de la commande
        try
        {
            order.Submit();
            await _unitOfWork.CommitAsync(cancellationToken);

            // Publication des événements de domaine
            foreach (var domainEvent in order.DomainEvents)
            {
                await _eventBus.PublishAsync(domainEvent, cancellationToken);
            }
            order.ClearDomainEvents();

            _logger.LogInformation("Order {OrderId} submitted successfully with reservation {ReservationId}",
                order.Id, reservationId);

            return new SubmitOrderResult(true, "Order submitted successfully");
        }
        catch (Exception ex)
        {
            // Rollback des réservations en cas d'erreur
            await _inventoryService.CancelReservationAsync(reservationId, cancellationToken);
            await _paymentService.CancelPreAuthorizationAsync(preAuthResult.AuthorizationId, cancellationToken);

            _logger.LogError(ex, "Failed to submit order {OrderId}", order.Id);
            throw;
        }
    }
}

