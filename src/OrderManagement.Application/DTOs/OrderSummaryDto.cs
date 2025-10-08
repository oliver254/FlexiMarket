namespace FlexiMarket.OrderManagement.Application.DTOs;

public record OrderSummaryDto(Guid Id, string OrderNumber, string Status, decimal TotalAmount);
