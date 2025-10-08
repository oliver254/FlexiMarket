using FlexiMarket.SharedKernel;

namespace FlexiMarket.OrderManagement.Domain.ValueObjects;

public class CustomerId : ValueObject
{
    public Guid Value { get; private set; }

    private CustomerId() { }

    public CustomerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty");
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(CustomerId customerId) => customerId.Value;
}
