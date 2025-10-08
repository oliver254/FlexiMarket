using FlexiMarket.OrderManagement.Domain.ValueObjects;
using FlexiMarket.SharedKernel;

namespace FlexiMarket.OrderManagement.Domain.Entities;

public class OrderLine : Entity
{
    public Guid Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money LineTotal { get; private set; }

    private OrderLine() { }

    public static OrderLine Create(ProductId productId, int quantity, Money unitPrice)
    {
        var line = new OrderLine
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
        line.CalculateLineTotal();
        return line;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        Quantity = newQuantity;
        CalculateLineTotal();
    }

    private void CalculateLineTotal()
    {
        LineTotal = new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);
    }
}
