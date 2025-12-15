namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Security;

/// <summary>
/// Команда продления лицензии пользователя.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="ExpirationDateTime"> Дата и время истечения подписки. </param>
public sealed record ExtendLicenseCommand(
    Guid OperationId,
    Guid UserId,
    DateTimeOffset ExpirationDateTime
);
