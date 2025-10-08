using FlexiMarket.OrderManagement.Application.DTOs;
using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface IInventoryService
{
    Task<bool> CheckAvailabilityAsync(ProductId productId, int quantity, CancellationToken cancellationToken);
    Task<string> ReserveInventoryAsync(Guid orderId, List<InventoryReservationItem> items, CancellationToken cancellationToken);
    Task CancelReservationAsync(string reservationId, CancellationToken cancellationToken);
}
