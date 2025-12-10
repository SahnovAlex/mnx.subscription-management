using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

namespace MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling;

/// <summary>
/// Реализация <see cref="ISubscriptionStrategyFactory"/>.
/// </summary>
public class SubscriptionStrategyFactory : ISubscriptionStrategyFactory
{
    /// <summary>
    /// Словарь существующих семейств стратегий.
    /// </summary>
    private readonly Dictionary<PaymentStrategyType, ISubscriptionStrategyFamily> _families;

    public SubscriptionStrategyFactory(IEnumerable<ISubscriptionStrategyFamily> families)
    {
        _families = families.ToDictionary(x => x.PaymentStrategyType);
    }

    /// <inheritdoc/>
    public ISubscriptionStrategyFamily GetStrategyFamily(PaymentStrategyType strategy)
    {
        return _families[strategy];
    }
}
