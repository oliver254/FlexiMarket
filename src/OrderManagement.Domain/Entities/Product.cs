using FlexiMarket.OrderManagement.Domain.ValueObjects;

namespace FlexiMarket.OrderManagement.Domain.Entities;

public class Product
{
    public ProductId Id { get; set; }
    public string Name { get; set; }
    public bool IsAvailable { get; set; }
}