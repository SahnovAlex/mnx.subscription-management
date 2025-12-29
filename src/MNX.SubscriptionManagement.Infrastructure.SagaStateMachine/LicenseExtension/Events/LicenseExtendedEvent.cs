namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.LicenseExtension.Events;

public sealed record LicenseExtendedEvent(
    Guid SubscriptionId,
    Guid UserId,
    DateTimeOffset ExpirationDate
);
