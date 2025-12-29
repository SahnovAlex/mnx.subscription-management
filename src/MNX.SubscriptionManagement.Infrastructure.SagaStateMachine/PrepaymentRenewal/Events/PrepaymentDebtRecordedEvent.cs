namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal.Events;

public sealed record PrepaymentDebtRecordedEvent(
    Guid SubscriptionId,
    Guid UserId
);
