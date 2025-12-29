using MassTransit;
using MNX.SecurityManagement.Licensing.Contracts;
using MNX.SubscriptionManagement.Application.SagaInitiation;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Security;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.LicenseExtension.Events;

namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.LicenseExtension;

public sealed class LicenseExtensionSaga : MassTransitStateMachine<LicenseExtensionSagaState>
{
    public State LicenseExtending { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<LicenseExtensionInitiated> StartSaga { get; private set; }
    public Event<SuccessfullyLicenseExtendedMessage> LicenseExtended { get; private set; }
    public Event<UnsuccessfullyLicenseExtendedMessage> LicenseExtensionFailed { get; private set; }

    public LicenseExtensionSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartSaga, x =>
        {
            x.CorrelateById(x => x.Message.OperationId);
            x.InsertOnInitial = true;
        });
        Event(() => LicenseExtended, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => LicenseExtensionFailed, x => x.CorrelateById(x => x.Message.OperationId));

        Initially(When(StartSaga).Then(context =>
        {
            context.Saga.UserId = new UserId(context.Message.UserId);
            context.Saga.ExpirationDate = context.Message.ExpirationDate;
            context.Saga.CreatedAt = DateTime.UtcNow;
        }).TransitionTo(LicenseExtending)
        .Publish(context => new ExtendLicenseCommand(
            context.Saga.CorrelationId,
            context.Saga.UserId,
            context.Saga.ExpirationDate))
        );

        During(LicenseExtending, When(LicenseExtended)
            .Publish(context => new LicenseExtendedEvent(
                context.Saga.SubscriptionId,
                context.Saga.UserId,
                context.Saga.ExpirationDate
            )).TransitionTo(Completed)
            .Finalize(),
            When(LicenseExtensionFailed)
            .Publish(context => new LicenseExtensionFailedEvent(
                context.Saga.SubscriptionId,
                context.Saga.UserId
            )).TransitionTo(Failed)
            .Finalize());
    }
}
