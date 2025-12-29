using MassTransit;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal;

public sealed class PrepaymentSubscriptionRenewalSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;

    public SubscriptionId SubscriptionId { get; set; }
    public UserId UserId { get; set; }
    public float Amount { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
