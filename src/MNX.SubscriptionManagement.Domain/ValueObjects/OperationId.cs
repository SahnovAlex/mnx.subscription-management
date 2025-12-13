using System.Diagnostics;

namespace MNX.SubscriptionManagement.Domain.Core.ValueObjects;

/// <summary>
/// Идентификатор транзакционной операции
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct OperationId
{
    public Guid Value { get; }

    public OperationId()
    {
        Value = Guid.NewGuid();
    }

    public OperationId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Operation id cannot be empty", nameof(value));
        Value = value;
    }

    public static implicit operator Guid(OperationId id) => id.Value;
    public static explicit operator OperationId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
