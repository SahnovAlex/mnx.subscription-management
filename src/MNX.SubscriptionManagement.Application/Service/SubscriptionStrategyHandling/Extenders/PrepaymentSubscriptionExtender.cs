using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

namespace MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling.Extenders;

/// <summary>
/// Реализация <see cref="ISubscriptionExtender"/>.
/// </summary>
public class PrepaymentSubscriptionExtender : ISubscriptionExtender
{
    /// <inheritdoc/>
    public Task<bool> Extend(UserId userId, TariffId tariffId)
    {
        throw new NotImplementedException();
    }
}
