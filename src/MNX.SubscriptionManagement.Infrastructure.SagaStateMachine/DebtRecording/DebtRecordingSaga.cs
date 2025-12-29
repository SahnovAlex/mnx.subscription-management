using MassTransit;
using MNX.SubscriptionManagement.Application.SagaInitiation;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Payment;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.DebtRecording.Events;

namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.DebtRecording;

public sealed class DebtRecordingSaga : MassTransitStateMachine<DebtRecordingSagaState>
{
    public State DebtRecording { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<DebtRecordingInitiated> StartSaga { get; private set; }
    public Event<SuccessfullyWrittenOffMessage> SuccessfullyDebtRecorded { get; private set; }
    public Event<UnsuccessfullyWrittenOffMessage> UnsuccessfullyDebtRecorded { get; private set; }

    public DebtRecordingSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartSaga, x =>
        {
            x.CorrelateById(x => x.Message.OperationId);
            x.InsertOnInitial = true;
        });
        Event(() => SuccessfullyDebtRecorded, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => UnsuccessfullyDebtRecorded, x => x.CorrelateById(x => x.Message.OperationId));

        Initially(When(StartSaga).Then(context =>
        {
            context.Saga.UserId = new UserId(context.Message.UserId);
            context.Saga.Amount = context.Message.Amount;
        }).TransitionTo(DebtRecording)
        .Publish(context => new WriteOffFundsCommand(
            new OperationId(context.Saga.CorrelationId),
            context.Saga.Amount,
            IsForced: true))
        );

        During(DebtRecording, When(SuccessfullyDebtRecorded)
            .Publish(context => new DebtRecordedEvent(
                context.Saga.SubscriptionId,
                context.Saga.UserId))
            .TransitionTo(Completed)
            .Finalize(),
        When(UnsuccessfullyDebtRecorded)
            .IfElse(context => context.Saga.Retries < 3,
                x => x.Then(context => context.Saga.Retries++)
                .Publish(context => new WriteOffFundsCommand(
                    new OperationId(context.Saga.CorrelationId),
                    context.Saga.Amount,
                    IsForced: true)
                ).TransitionTo(DebtRecording),
                x => x.Publish(context => new DebtRecordingFailed(
                        context.Saga.CorrelationId,
                        context.Saga.CreatedAt,
                        Reason: "Debt recording failed 3 times")
                ).TransitionTo(Failed)
            .Finalize())
        );
    }
}
