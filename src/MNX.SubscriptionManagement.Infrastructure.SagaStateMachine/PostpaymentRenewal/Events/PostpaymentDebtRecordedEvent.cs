namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal.Events;

public sealed record PostpaymentDebtRecordedEvent(
    Guid SubscriptionId,
    Guid UserId
);
