using MassTransit;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.DebtRecording;

public class DebtRecordingSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public SubscriptionId SubscriptionId { get; set; }
    public UserId UserId { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTime.Now;
    public float Amount { get; set; }
    public int Retries { get; set; } = 0;
}
