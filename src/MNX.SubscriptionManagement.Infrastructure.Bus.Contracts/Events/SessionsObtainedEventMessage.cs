namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Events;

public sealed record SessionsObtainedEventMessage(
    Guid OperationId,
    Guid SubscriptionId,
    Guid UserId,
    List<string> AgentSessions
);
