using MNX.SubscriptionManagement.Domain.SagaOperations.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.SagaOperations.Actions;

public abstract class SagaAction
{
    public SagaActionId Id { get; init; }


}
