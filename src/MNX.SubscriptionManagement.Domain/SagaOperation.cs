using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Core;

/// <summary>
/// Операция саги.
/// </summary>
public class SagaOperation
{
    /// <summary>
    /// Идентификатор процесса продления подписки.
    /// </summary>
    public OperationId Id { get; init; }

    /// <summary>
    /// Описание причины ошибки операции.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Статус процесса продления подписки.
    /// </summary>
    public OperationStatus Status { get; set; }
}
