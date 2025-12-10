using System.Diagnostics;

namespace MNX.SubscriptionManagement.Domain.Core.ValueObjects;

/// <summary>
/// Идентификатор тарифного плана.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct TariffId
{
    public Guid Value { get; }

    public TariffId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Subscription id cannot be empty", nameof(value));
        Value = value;
    }

    public static implicit operator Guid(TariffId id) => id.Value;
    public static explicit operator TariffId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
