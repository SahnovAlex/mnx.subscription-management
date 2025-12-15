using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces;

/// <summary>
/// Планировщик оплаты.
/// </summary>
public interface IPaymentScheduler
{
    Task<bool> Exists(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить расписание.
    /// </summary>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="subscriptionId"> Идентификатор подписки. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task AddSchedule(UserId userId, SubscriptionId subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить расписание.
    /// </summary>
    /// <param name="subscriptionId"> Идентификатор подписки. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task DeleteSchedule(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);
}
