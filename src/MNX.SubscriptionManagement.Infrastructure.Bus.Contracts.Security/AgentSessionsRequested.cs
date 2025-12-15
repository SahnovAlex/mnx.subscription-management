namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Security;

public sealed record AgentSessionsRequested(
    Guid OperationId,
    Guid UserId,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime
);
