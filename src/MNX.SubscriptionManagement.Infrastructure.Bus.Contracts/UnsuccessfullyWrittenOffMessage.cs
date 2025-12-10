namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

/// <summary>
/// Сообщение о неудачном списании средств.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="FailureReason"> Причина провала операции. </param>
public sealed record UnsuccessfullyWrittenOffMessage(Guid OperationId, string FailureReason);
