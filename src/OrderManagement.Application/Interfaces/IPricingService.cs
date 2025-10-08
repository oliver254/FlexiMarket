using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface IPricingService
{
    Task<Money> GetCurrentPriceAsync(ProductId productId, CancellationToken cancellationToken);
}
