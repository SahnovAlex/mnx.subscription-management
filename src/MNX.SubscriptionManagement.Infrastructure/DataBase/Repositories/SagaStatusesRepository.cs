using Microsoft.EntityFrameworkCore;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase.Repositories;

/// <summary>
/// Реализация <see cref="ISagaStatusesRepository"/>.
/// </summary>
public class SagaStatusesRepository : ISagaStatusesRepository
{
    private readonly Context _context;

    public SagaStatusesRepository(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public Task<SagaOperation?> GetById(OperationId operationId, CancellationToken cancellationToken = default)
    {
        return _context.SagaOperationStatuses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<SagaOperation> Add(SagaOperation operationStatus, CancellationToken cancellationToken = default)
    {
        await _context.SagaOperationStatuses.AddAsync(operationStatus, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return operationStatus;
    }

    /// <inheritdoc/>
    public Task Update(SagaOperation operation, CancellationToken cancellationToken = default)
    {
        _context.SagaOperationStatuses.Update(operation);
        return _context.SaveChangesAsync(cancellationToken);
    }
}
