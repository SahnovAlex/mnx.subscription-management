namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal.Events;

public sealed record PrepaymentDebtRecordingFailedEvent(
    Guid SubscriptionId,
    Guid UserId,
    float Amount,
    Guid? PreviousOperationId
);
