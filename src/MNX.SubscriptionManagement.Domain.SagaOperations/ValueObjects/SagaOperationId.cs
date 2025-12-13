namespace MNX.SubscriptionManagement.Domain.SagaOperations.ValueObjects;

public readonly struct SagaOperationId
{
    public readonly Guid Value;

    public SagaOperationId()
    {
        Value = Guid.NewGuid();
    }

    public SagaOperationId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentNullException(nameof(value), "Saga operation id cannot be null or empty");
        Value = value;
    }

    public static explicit operator SagaOperationId(Guid value) => new(value);
    public static implicit operator Guid(SagaOperationId id) => id.Value;

    public override string ToString() => Value.ToString();
}
