using FlexiMarket.SharedKernel.Interfaces;

namespace FlexiMarket.SharedKernel;

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}
