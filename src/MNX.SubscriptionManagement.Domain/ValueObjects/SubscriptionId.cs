using System.Diagnostics;

namespace MNX.SubscriptionManagement.Domain.Core.ValueObjects;

/// <summary>
/// Идентификатор подписки.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct SubscriptionId
{
    public Guid Value { get; }

    public SubscriptionId()
    {
        Value = Guid.NewGuid();
    }

    public SubscriptionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Subscription id cannot be empty", nameof(value));
        Value = value;
    }

    public static implicit operator Guid(SubscriptionId id) => id.Value;
    public static explicit operator SubscriptionId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
