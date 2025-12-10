namespace MNX.SubscriptionManagement.Domain.Core.Enums;

/// <summary>
/// Статус операции.
/// </summary>
public enum OperationStatus
{
    LicenseExtending,

    LicenseExtended,

    SubscriptionExpired,

    Success,

    Fail
}
