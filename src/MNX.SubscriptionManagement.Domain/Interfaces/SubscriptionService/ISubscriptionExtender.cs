using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

/// <summary>
/// Интерфейс продления подписки.
/// </summary>
public interface ISubscriptionExtender
{
    /// <summary>
    /// Обработать продление подписки.
    /// </summary>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="tariffId"> Идентификатор тарифного плана. </param>
    /// <returns> Результат продления подписки. </returns>
    Task<bool> Extend(UserId userId, TariffId tariffId);
}
