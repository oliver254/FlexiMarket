using FlexiMarket.OrderManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiMarket.OrderManagement.Infrastructure.Persistence;

/// <summary>
/// DbContext pour le microservice de gestion des commandes.
/// Respecte les principes DDD avec des configurations explicites.
/// </summary>
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    // DbSets pour les agrégats
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Appliquer toutes les configurations depuis l'assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);

        // Ou appliquer manuellement
        // modelBuilder.ApplyConfiguration(new OrderConfiguration());
        // modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
    }

    // Méthode pour sauvegarder avec gestion des événements de domaine
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Optionnel : Publier les événements de domaine avant de sauvegarder
        // await DispatchDomainEventsAsync();

        return await base.SaveChangesAsync(cancellationToken);
    }
}

