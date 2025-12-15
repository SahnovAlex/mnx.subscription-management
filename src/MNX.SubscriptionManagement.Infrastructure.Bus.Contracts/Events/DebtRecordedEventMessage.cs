namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Events;

public sealed record DebtRecordedEventMessage(
    Guid SubscriptionId,
    Guid UserId
);
