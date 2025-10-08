using FlexiMarket.OrderManagement.Application.Interfaces;

// ============================================================================
// UNIT OF WORK - Implémentation
// ============================================================================

// Infrastructure/Persistence/UnitOfWork.cs
namespace FlexiMarket.OrderManagement.Infrastructure.Persistence;

/// <summary>
/// Implémentation du pattern Unit of Work avec EF Core.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;

    public UnitOfWork(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
