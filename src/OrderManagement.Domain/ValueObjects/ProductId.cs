using FlexiMarket.SharedKernel;

namespace FlexiMarket.OrderManagement.Domain.ValueObjects;

public class ProductId : ValueObject
{
    public Guid Value { get; private set; }

    private ProductId() { }

    public ProductId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty");
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(ProductId productId) => productId.Value;
}
