using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

/// <summary>
/// Интерфейс оформления подписки.
/// </summary>
public interface ISubscriptionActivator
{
    /// <summary>
    /// Обработать оформление подписки.
    /// </summary>
    /// <param name="operationId"> Идентификатор операции. </param>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="tariffPlan"> Тарифный план. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <returns> Результат оформления подписки и экземпляр подписки. </returns>
    /// <remarks>
    /// Считается успешной, если средства за подписку успешно заморожены
    /// и подписка добавлена в базу данных с триггером.
    /// </remarks>
    Task<(bool Result, Subscription? Subscription)> ProcessAsync(OperationId operationId,
                                                                 UserId userId,
                                                                 TariffPlan tariffPlan,
                                                                 CancellationToken cancellationToken = default);
}
