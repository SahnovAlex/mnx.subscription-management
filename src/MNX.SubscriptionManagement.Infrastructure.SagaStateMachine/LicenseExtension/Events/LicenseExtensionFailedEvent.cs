namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.LicenseExtension.Events;

public sealed record LicenseExtensionFailedEvent(
    Guid SubscriptionId,
    Guid UserId
);
