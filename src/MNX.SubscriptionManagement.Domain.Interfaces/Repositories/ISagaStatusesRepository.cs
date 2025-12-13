using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий управления статусом операции продления подписки.
/// </summary>
public interface ISagaStatusesRepository
{
    /// <summary>
    /// Получить статус операции обновления подписки.
    /// </summary>
    /// <param name="operationId"> Идентификатор операции. </param>
    /// <returns> Текущий статус операции продления. </returns>
    Task<SagaOperation?> GetById(OperationId operationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Инициировать процесс продления подписки.
    /// </summary>
    /// <param name="operationStatus"> Статус операции. </param>
    /// <returns> Текущий статус операции продления. </returns>
    Task<SagaOperation> Add(SagaOperation operationStatus, CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактировать статус операции обновления подписки.
    /// </summary>
    /// <param name="operation"> Операция продления. </param>
    Task Update(SagaOperation operation, CancellationToken cancellationToken = default);
}
