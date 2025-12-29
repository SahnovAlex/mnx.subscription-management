using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Application.SagaInitiation;

/// <summary>
/// Сообщение о начале операции продления подписки.
/// </summary>
/// <remarks>
/// Инициирует операцию саги, в которой две операции:
/// продление лицензии и фиксация долга.
/// </remarks>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="ExpirationDate"> Дата окончания срока подписки. </param>
/// <param name="Amount"> Сумма оплаты подписки. </param>
public sealed record RenewalSubscriptionInitiated(
    OperationId OperationId,
    UserId UserId,
    DateTimeOffset ExpirationDate,
    float Amount
);
