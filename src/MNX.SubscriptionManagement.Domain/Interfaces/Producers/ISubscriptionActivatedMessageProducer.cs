using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Producers;

/// <summary>
/// Интерфейс производителя сообщений о том, что подписка была активирована.
/// </summary>
public interface ISubscriptionActivatedMessageProducer
{
    /// <summary>
    /// Произвести публикацию сообщения.
    /// </summary>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="operationId"> Идентификатор операции. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Produce(UserId userId, OperationId operationId, CancellationToken cancellationToken = default);
}
