namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

/// <summary>
/// Сообщение о продлении лицензии пользователя.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="IsForced"> Признак фиксации пользовательского долга. </param>
public sealed record LicenseExtendedMessage(Guid OperationId, bool IsForced);
