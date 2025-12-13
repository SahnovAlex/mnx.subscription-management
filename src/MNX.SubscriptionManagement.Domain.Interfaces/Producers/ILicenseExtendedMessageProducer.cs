using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Producers;

/// <summary>
/// Интерфейс публикатора сообщений об истечении лицензии.
/// </summary>
public interface ILicenseExtendedMessageProducer
{
    /// <summary>
    /// Опубликовать сообщение.
    /// </summary>
    /// <param name="operationId"> Идентификатор операции. </param>
    /// <param name="isForced"> Признак принудительного списания. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Produce(OperationId operationId, bool isForced, CancellationToken cancellationToken = default);
}
