using MNX.SubscriptionManagement.Domain.Core.Enums;

namespace MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

/// <summary>
/// Фабрика стратегий оплаты подписки.
/// </summary>
public interface ISubscriptionStrategyFactory
{
    /// <summary>
    /// Предоставить семейство стратегий продления и оформления
    /// подписки в соответствии с типом тарифного плана.
    /// </summary>
    /// <param name="strategy"> Стратегия оплаты подписки. </param>
    /// <returns> Семейство стратегий взаимодействия с подпиской. </returns>
    ISubscriptionStrategyFamily GetStrategyFamily(PaymentStrategyType strategy);
}
