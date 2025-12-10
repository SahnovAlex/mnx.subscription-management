using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase.Repositories;

/// <summary>
/// Реализация <see cref="ISubscriptionWithSagaRepository"/>.
/// </summary>
public class SubscriptionWithSagaRepository : ISubscriptionWithSagaRepository
{
    private readonly Context _context;

    public SubscriptionWithSagaRepository(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public Task Update(SagaOperation operation, Subscription subscription, CancellationToken cancellationToken = default)
    {
        _context.SagaOperationStatuses.Update(operation);
        _context.Subscriptions.Update(subscription);
        return _context.SaveChangesAsync(cancellationToken);
    }
}
