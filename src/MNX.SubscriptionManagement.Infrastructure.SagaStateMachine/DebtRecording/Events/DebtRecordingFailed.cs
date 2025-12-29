namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.DebtRecording.Events;

public sealed record DebtRecordingFailed(
    Guid OperationId,
    DateTimeOffset CreatedAt,
    string Reason
);