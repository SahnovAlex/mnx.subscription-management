using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Producers;

/// <summary>
/// Интерфейс производителя сообщений о том, что подписка была создана.
/// </summary>
public interface ISubscriptionCreatedMessageProducer
{
    /// <summary>
    /// Произвести публикацию сообщения.
    /// </summary>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="operationId"> Идентификатор операции. </param>
    /// <param name="ExpirationDateTime"> Дата и время истечения подписки. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Produce(UserId userId, OperationId operationId, DateTimeOffset ExpirationDateTime, CancellationToken cancellationToken = default);
}
