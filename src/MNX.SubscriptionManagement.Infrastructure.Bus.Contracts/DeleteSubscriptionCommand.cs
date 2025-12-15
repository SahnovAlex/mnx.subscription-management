namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

/// <summary>
/// Команда удаления подписки.
/// </summary>
/// <param name="SubscriptionId"> Идентификатор подписки. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
public sealed record DeleteSubscriptionCommand(
    Guid SubscriptionId,
    Guid UserId
);
