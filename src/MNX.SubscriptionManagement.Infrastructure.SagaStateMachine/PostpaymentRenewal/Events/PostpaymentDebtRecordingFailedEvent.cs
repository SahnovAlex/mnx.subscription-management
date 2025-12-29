namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal.Events;

public sealed record PostpaymentDebtRecordingFailedEvent(
    Guid SubscriptionId,
    Guid UserId,
    float Amount,
    Guid? PreviousOperationId
);
