namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Events;

public sealed record LicenseExtendedEventMessage(
    Guid SubscriptionId,
    Guid UserId,
    DateTimeOffset ExpirationDate
);
