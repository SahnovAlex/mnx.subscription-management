using Microsoft.EntityFrameworkCore;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase.Repositories;

/// <summary>
/// Реализация <see cref="ISubscriptionRepository"/>.
/// </summary>
public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly Context _context;

    public SubscriptionRepository(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public Task<Subscription?> GetById(
        SubscriptionId id,
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Subscriptions.AsNoTracking()
                       .Include(x => x.TariffPlan)
                       .Where(x => x.UserId == userId)
                       .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Subscription?> GetCurrent(UserId userId, CancellationToken cancellationToken = default)
    {
        return _context.Subscriptions.AsNoTracking()
                       .Include(x => x.TariffPlan)
                       .Where(x => x.UserId == userId)
                       .FirstOrDefaultAsync(x => x.Status == SubscriptionStatus.Active, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task Create(Subscription subscription, CancellationToken cancellationToken = default)
    {
        await _context.Subscriptions.AddAsync(subscription, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task Update(Subscription subscription, CancellationToken cancellationToken = default)
    {
        _context.Subscriptions.Update(subscription);
        return _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task Delete(SubscriptionId subscriptionId, CancellationToken cancellationToken = default)
    {
        return _context.Subscriptions.Where(x => x.Id == subscriptionId)
                                     .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }
}
