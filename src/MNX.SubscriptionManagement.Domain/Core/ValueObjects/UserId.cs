using System.Diagnostics;

namespace MNX.SubscriptionManagement.Domain.Core.ValueObjects;

/// <summary>
/// Идентификатор пользователя.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct UserId
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("User id cannot be empty", nameof(value));
        Value = value;
    }

    public static implicit operator Guid(UserId id) => id.Value;
    public static explicit operator UserId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
