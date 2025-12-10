using Microsoft.EntityFrameworkCore;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase.Repositories;

/// <summary>
/// Реализация <see cref="ITariffPlanRepository"/>.
/// </summary>
public class TariffPlanRepository : ITariffPlanRepository
{
    private readonly Context _context;

    public TariffPlanRepository(Context context)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TariffPlan>> GetAll()
    {
        return await _context.TariffPlans.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc/>
    public Task<bool> Exists(TariffId tariffId, CancellationToken cancellationToken = default)
    {
        return _context.TariffPlans.AnyAsync(x => x.Id == tariffId.Value, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TariffPlan?> GetById(TariffId id, CancellationToken cancellationToken = default)
    {
        return _context.TariffPlans.AsNoTracking()
                                   .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }
}
