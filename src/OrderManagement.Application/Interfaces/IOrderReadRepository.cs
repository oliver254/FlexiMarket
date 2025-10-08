using FlexiMarket.OrderManagement.Application.DTOs;
using FlexiMarket.OrderManagement.Application.UseCases.Queries;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface IOrderReadRepository
{
    Task<OrderDetailsDto> GetOrderDetailsAsync(Guid orderId, CancellationToken cancellationToken);
    Task<PagedResult<OrderSummaryDto>> GetOrdersAsync(OrderSearchCriteria criteria, CancellationToken cancellationToken);
}
