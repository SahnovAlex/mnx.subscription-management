using MassTransit;
using MNX.SecurityManagement.Licensing.Contracts;
using MNX.SubscriptionManagement.Application.SagaInitiation;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Payment;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Security;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal.Events;

namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal;

public sealed class PrepaymentSubscriptionRenewalSaga : MassTransitStateMachine<PrepaymentSubscriptionRenewalSagaState>
{
    public State LicenseExtending { get; private set; }
    public State DebtRecording { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<RenewalSubscriptionInitiated> StartSaga { get; private set; }
    public Event<SuccessfullyLicenseExtendedMessage> LicenseExtended { get; private set; }
    public Event<UnsuccessfullyLicenseExtendedMessage> LicenseExtensionFailed { get; private set; }

    public Event<SuccessfullyWrittenOffMessage> SuccessfullyDebtRecorded { get; private set; }
    public Event<UnsuccessfullyWrittenOffMessage> UnsuccessfullyDebtRecorded { get; private set; }

    public PrepaymentSubscriptionRenewalSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartSaga, x =>
        {
            x.CorrelateById(x => x.Message.OperationId);
            x.InsertOnInitial = true;
        });
        Event(() => LicenseExtended, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => LicenseExtensionFailed, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => SuccessfullyDebtRecorded, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => UnsuccessfullyDebtRecorded, x => x.CorrelateById(x => x.Message.OperationId));

        Initially(When(StartSaga).Then(context =>
        {
            context.Saga.UserId = new UserId(context.Message.UserId);
            context.Saga.ExpirationDate = context.Message.ExpirationDate;
            context.Saga.CreatedAt = DateTime.UtcNow;
        }).Publish(context => new ExtendLicenseCommand(
            context.Saga.CorrelationId,
            context.Saga.UserId,
            context.Saga.ExpirationDate)
        ).TransitionTo(LicenseExtending));

        During(LicenseExtending, When(LicenseExtended)
            .Publish(context => new WriteOffFundsCommand(
                new OperationId(context.Saga.CorrelationId),
                context.Saga.Amount,
                IsForced: true
            )).TransitionTo(DebtRecording),
        When(LicenseExtensionFailed)
            .Publish(context => new PrepaymentLicenseExtensionFailedEvent(
                context.Saga.SubscriptionId,
                context.Saga.UserId
            ))
            .TransitionTo(Failed)
            .Finalize()
        );

        During(DebtRecording, When(SuccessfullyDebtRecorded)
            .Publish(context => new PrepaymentDebtRecordedEvent(
                context.Saga.SubscriptionId,
                context.Saga.UserId
            )).TransitionTo(Completed)
            .Finalize(), When(UnsuccessfullyDebtRecorded)
            .Publish(context => new PrepaymentDebtRecordingFailedEvent(
                new OperationId(Guid.NewGuid()),
                context.Saga.UserId,
                context.Saga.Amount,
                new OperationId(context.Saga.CorrelationId)
            )).TransitionTo(Failed)
            .Finalize()
        );
    }
}
