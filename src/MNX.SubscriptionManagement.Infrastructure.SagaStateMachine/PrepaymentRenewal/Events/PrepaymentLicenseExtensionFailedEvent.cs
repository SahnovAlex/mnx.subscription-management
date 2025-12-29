namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal.Events;

public sealed record PrepaymentLicenseExtensionFailedEvent(
    Guid SubscriptionId,
    Guid UserId
);
