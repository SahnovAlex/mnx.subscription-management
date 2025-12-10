using MNX.SubscriptionManagement.Domain.Core;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий данных сущностей <see cref="Subscription"/> и <see cref="SagaOperation"/>.
/// </summary>
public interface ISubscriptionWithSagaRepository
{
    /// <summary>
    /// Обновить статус подписки и операции саги согласованно.
    /// </summary>
    /// <param name="operation"> Операция саги. </param>
    /// <param name="subscription"> Подписка. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    Task Update(SagaOperation operation, Subscription subscription, CancellationToken cancellationToken = default);
}
