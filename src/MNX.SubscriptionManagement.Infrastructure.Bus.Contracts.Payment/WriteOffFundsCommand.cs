namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Payment;

/// <summary>
/// Сообщение о продлении лицензии пользователя.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="Amount"> Сумма списания. </param>
/// <param name="IsForced"> Признак фиксации пользовательского долга. </param>
public sealed record WriteOffFundsCommand(Guid OperationId, float Amount, bool IsForced);
