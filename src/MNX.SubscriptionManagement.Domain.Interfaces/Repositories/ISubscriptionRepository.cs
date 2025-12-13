using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий с подписками.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>
    /// Получить текущую подписку.
    /// </summary>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <returns> Текущая подписка. </returns>
    Task<Subscription?> GetCurrent(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить подписку по идентификатору.
    /// </summary>
    /// <param name="id"> Идентификатор подписки. </param>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <returns> Подписка. </returns>
    Task<Subscription?> GetById(SubscriptionId id, UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать подписку.
    /// </summary>
    /// <param name="subscription"> Подписка. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Create(Subscription subscription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить подписку.
    /// </summary>
    /// <param name="subscription"> Подписка. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Update(Subscription subscription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить подписку.
    /// </summary>
    /// <param name="subscriptionId"> Идентификатор подписки. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Delete(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);
}
