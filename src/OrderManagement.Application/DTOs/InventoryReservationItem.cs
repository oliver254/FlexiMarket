using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Application.DTOs;

public record InventoryReservationItem(ProductId ProductId, int Quantity);
