namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.DebtRecording.Events;

public sealed record DebtRecordedEvent(
    Guid SubscriptionId,
    Guid UserId
);
