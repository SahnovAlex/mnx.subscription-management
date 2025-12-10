namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

/// <summary>
/// Сообщение об успешном создании подписки пользователя.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="ExpirationDateTime"> Дата и время истечения подписки. </param>
public sealed record SubscriptionCreatedMessage(Guid OperationId, Guid UserId, DateTimeOffset ExpirationDateTime);
