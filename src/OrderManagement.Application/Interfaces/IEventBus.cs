using FlexiMarket.SharedKernel.Interfaces;

namespace FlexiMarket.OrderManagement.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
