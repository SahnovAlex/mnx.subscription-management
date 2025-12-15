using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Application.SagaInitiation;

/// <summary>
/// Сообщение о начале операции фиксации долга за подписку.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="Amount"> Сумма долга за подписку. </param>
public sealed record DebtRecordingInitiated(
    OperationId OperationId,
    UserId UserId,
    float Amount
);
