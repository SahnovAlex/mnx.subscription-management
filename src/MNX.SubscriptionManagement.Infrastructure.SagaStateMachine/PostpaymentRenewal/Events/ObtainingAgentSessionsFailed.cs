namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal.Events;

public sealed record ObtainingAgentSessionsFailed(
    Guid OperationId,
    DateTimeOffset CreatedAt,
    string Reason
);
