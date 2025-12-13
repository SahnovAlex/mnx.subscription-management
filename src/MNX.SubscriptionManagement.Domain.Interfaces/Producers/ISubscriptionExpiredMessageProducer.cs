using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Producers;

/// <summary>
/// Интерфейс производителя сообщений о том, что подписка была продлена.
/// </summary>
public interface ISubscriptionExpiredMessageProducer
{
    /// <summary>
    /// Произвести публикацию сообщения.
    /// </summary>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="operationId"> Идентификатор операции. </param>
    /// <param name="subscriptionId"> Идентификатор подписки. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Produce(UserId userId, OperationId operationId, SubscriptionId subscriptionId, CancellationToken cancellationToken = default);
}
