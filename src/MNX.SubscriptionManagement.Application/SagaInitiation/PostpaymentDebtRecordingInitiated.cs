using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Application.SagaInitiation;

/// <summary>
/// Сообщение о начале операции расчёта и фиксации долга за 
/// подписку, оформленную по постоплате.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="StartDateTime"> Начальный расчётный период. </param>
/// <param name="EndDateTime"> Конечный расчётный период. </param>
public sealed record PostpaymentDebtRecordingInitiated(
    OperationId OperationId,
    SubscriptionId SubscriptionId,
    UserId UserId,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime
);
