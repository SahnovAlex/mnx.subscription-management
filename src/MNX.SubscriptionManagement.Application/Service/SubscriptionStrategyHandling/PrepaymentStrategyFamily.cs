using MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling.Activators;
using MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling.Extenders;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

namespace MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling;

/// <summary>
/// Реализация <see cref="ISubscriptionStrategyFamily"/> для стратегии предоплаты подписки.
/// </summary>
public class PrepaymentStrategyFamily : ISubscriptionStrategyFamily
{
    public PrepaymentStrategyFamily(PrepaymentSubscriptionActivator formatter,
                                    PrepaymentSubscriptionExtender extender)
    {
        Activator = formatter ?? throw new ArgumentNullException(nameof(formatter));
        Extender = extender ?? throw new ArgumentNullException(nameof(extender));
    }

    /// <inheritdoc/>
    public PaymentStrategyType PaymentStrategyType => PaymentStrategyType.Prepayment;

    /// <inheritdoc/>
    public ISubscriptionActivator Activator { get; }

    /// <inheritdoc/>
    public ISubscriptionExtender Extender { get; }
}
