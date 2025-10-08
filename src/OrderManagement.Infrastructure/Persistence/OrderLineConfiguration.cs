using FlexiMarket.OrderManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexiMarket.OrderManagement.Infrastructure.Persistence;

/// <summary>
/// Configuration EF Core pour l'entité OrderLine.
/// </summary>
public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        // Table
        builder.ToTable("OrderLines");

        // Clé primaire
        builder.HasKey(ol => ol.Id);

        builder.Property(ol => ol.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Foreign Key vers Order (implicite via la relation)
        builder.Property<Guid>("OrderId")
            .IsRequired();

        builder.Property(ol => ol.Quantity)
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT : ProductId
        // ═══════════════════════════════════════════════════════════════
        builder.OwnsOne(ol => ol.ProductId, productId =>
        {
            productId.Property(p => p.Value)
                .HasColumnName("ProductId")
                .IsRequired();

            productId.HasIndex(p => p.Value);
        });

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT : Money (UnitPrice)
        // ═══════════════════════════════════════════════════════════════
        builder.OwnsOne(ol => ol.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT : Money (LineTotal)
        // ═══════════════════════════════════════════════════════════════
        builder.OwnsOne(ol => ol.LineTotal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("LineTotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("LineTotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // ═══════════════════════════════════════════════════════════════
        // INDEXES
        // ═══════════════════════════════════════════════════════════════
        builder.HasIndex("OrderId");
    }
}
