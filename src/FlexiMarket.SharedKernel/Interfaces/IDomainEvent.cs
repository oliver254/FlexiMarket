namespace FlexiMarket.SharedKernel.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
