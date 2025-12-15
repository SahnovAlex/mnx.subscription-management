namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Events;

public sealed record ObtainingAgentSessionsRequiresAttention(
    Guid OperationId,
    DateTimeOffset CreatedAt,
    string Reason
);
