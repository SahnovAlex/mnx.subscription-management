using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Application.SagaInitiation;

/// <summary>
/// Сообщение о начале операции продления лицензии.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="ExpirationDate"> Дата окончания срока подписки. </param>
public sealed record LicenseExtensionInitiated(
    OperationId OperationId,
    UserId UserId,
    DateTimeOffset ExpirationDate
);
