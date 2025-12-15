namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

/// <summary>
/// Команда инициации операции фиксации долга пользователя.
/// </summary>
/// <param name="SubscriptionId"> Идентификатор подписки. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="Amount"> Сумма долга. </param>
public sealed record InitiateDebtRecordingCommand(
    Guid SubscriptionId,
    Guid UserId,
    float Amount,
    Guid? PreviousOperationId
);
