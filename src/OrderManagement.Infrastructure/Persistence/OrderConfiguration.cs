using FlexiMarket.OrderManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexiMarket.OrderManagement.Infrastructure.Persistence;

/// <summary>
/// Configuration EF Core pour l'entité Order (Agrégat Racine).
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Table
        builder.ToTable("Orders");

        // Clé primaire
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.Id)
            .IsRequired()
            .ValueGeneratedNever(); // Guid généré par le domaine

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>() // Stocker l'enum comme string
            .HasMaxLength(50);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.CompletedAt)
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT : CustomerId
        // ═══════════════════════════════════════════════════════════════
        builder.OwnsOne(o => o.CustomerId, customerId =>
        {
            customerId.Property(c => c.Value)
                .HasColumnName("CustomerId")
                .IsRequired();

            customerId.HasIndex(c => c.Value);
        });

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT : Address (ShippingAddress)
        // ═══════════════════════════════════════════════════════════════
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("ShippingPostalCode")
                .HasMaxLength(20)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("ShippingCountry")
                .HasMaxLength(100)
                .IsRequired();
        });

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT : Money (TotalAmount)
        // ═══════════════════════════════════════════════════════════════
        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // ═══════════════════════════════════════════════════════════════
        // RELATION : Order → OrderLines (1-N)
        // ═══════════════════════════════════════════════════════════════
        builder.HasMany(o => o.OrderLines)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // ═══════════════════════════════════════════════════════════════
        // DOMAIN EVENTS - Ignore (non persistés)
        // ═══════════════════════════════════════════════════════════════
        builder.Ignore(o => o.DomainEvents);

        // ═══════════════════════════════════════════════════════════════
        // INDEXES pour les performances
        // ═══════════════════════════════════════════════════════════════
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}