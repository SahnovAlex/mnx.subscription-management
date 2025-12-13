using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий доступа к данным внешнего сервиса Payment.
/// </summary>
public interface IExternalPaymentRepository
{
    /// <summary>
    /// Попытаться заморозить средства за подписку на балансе пользователя.
    /// </summary>
    /// <param name="operationId"> Идентификатор операции. </param>
    /// <param name="userId"> Идентификатор пользователя. </param>
    /// <param name="amount"> Сумма к заморозке. </param>
    /// <returns> Признак успешности заморозки суммы на балансе пользователя. </returns>
    Task<bool> TryFreeze(OperationId operationId,
                         UserId userId,
                         float amount,
                         CancellationToken cancellationToken = default);
}