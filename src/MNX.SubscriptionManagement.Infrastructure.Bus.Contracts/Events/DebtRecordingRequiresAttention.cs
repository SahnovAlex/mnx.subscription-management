namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Events;

public sealed record DebtRecordingRequiresAttention(
    Guid OperationId,
    DateTimeOffset CreatedAt,
    string Reason
);
