namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

/// <summary>
/// Сообщение об окончании срока действия подписки.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="SubscriptionId"> Идентификатор подписки. </param>
public sealed record SubscriptionExpiredMessage(Guid OperationId, Guid UserId, Guid SubscriptionId);
