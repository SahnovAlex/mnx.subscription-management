namespace MNX.SubscriptionManagement.Domain.SagaOperations.ValueObjects;

public readonly struct SagaActionId
{
    public readonly Guid Value;

    public SagaActionId()
    {
        Value = Guid.NewGuid();
    }

    public SagaActionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentNullException(nameof(value), "Saga action id cannot be null or empty");
        Value = value;
    }

    public static explicit operator SagaActionId(Guid value) => new(value);
    public static implicit operator Guid(SagaActionId id) => id.Value;

    public override string ToString() => Value.ToString();
}
