using MNX.SubscriptionManagement.Domain.Core.Enums;

namespace MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

/// <summary>
/// Интерфейс, определяющий контракт для семейств стратегий работы подписки.
/// </summary>
public interface ISubscriptionStrategyFamily
{
    /// <summary>
    /// Тип стратегии оплаты подписки.
    /// </summary>
    PaymentStrategyType PaymentStrategyType { get; }

    /// <summary>
    /// Свойство реализаций форматеров подписки.
    /// </summary>
    ISubscriptionActivator Activator { get; }

    /// <summary>
    /// Свойство реализаций продлеваторов подписки.
    /// </summary>
    ISubscriptionExtender Extender { get; }
}
