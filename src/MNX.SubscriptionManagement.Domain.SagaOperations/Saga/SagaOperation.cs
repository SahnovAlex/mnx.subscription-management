using MNX.SubscriptionManagement.Domain.SagaOperations.Actions;
using MNX.SubscriptionManagement.Domain.SagaOperations.Enums;
using MNX.SubscriptionManagement.Domain.SagaOperations.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.SagaOperations.Saga;

public abstract class SagaOperation
{
    public SagaOperationId Id { get; init; }

    public OperationTypeEnum Type { get; }

    public abstract SagaAction Invoke();
}
