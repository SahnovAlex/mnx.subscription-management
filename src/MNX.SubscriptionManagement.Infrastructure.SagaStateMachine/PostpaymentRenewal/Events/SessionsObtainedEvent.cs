namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal.Events;

public sealed record SessionsObtainedEvent(
    Guid OperationId,
    Guid SubscriptionId,
    Guid UserId,
    List<string> AgentSessions
);
