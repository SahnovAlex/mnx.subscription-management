namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Payment;

/// <summary>
/// Сообщение с результатом списания замороженных средств.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
public sealed record SuccessfullyWrittenOffMessage(Guid OperationId);