namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Events;

public sealed record LicenseExtensionFailedEventMessage(
    Guid SubscriptionId,
    Guid UserId
);
