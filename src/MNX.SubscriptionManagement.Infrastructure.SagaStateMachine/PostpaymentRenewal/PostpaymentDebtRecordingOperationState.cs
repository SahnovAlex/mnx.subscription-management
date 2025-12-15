using MassTransit;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal;

public sealed class PostpaymentDebtRecordingOperationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;

    public SubscriptionId SubscriptionId { get; set; }
    public UserId UserId { get; set; }
    public float Amount { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTime.UtcNow;
    public int Retries { get; set; } = 0;
}