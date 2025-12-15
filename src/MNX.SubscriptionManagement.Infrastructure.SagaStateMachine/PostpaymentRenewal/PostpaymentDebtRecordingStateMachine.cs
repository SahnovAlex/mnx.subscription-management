using MassTransit;
using MNX.SubscriptionManagement.Application.SagaInitiation;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Events;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Payment;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Security;

namespace MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal;

public sealed class PostpaymentDebtRecordingStateMachine : MassTransitStateMachine<PostpaymentDebtRecordingOperationState>
{
    public State ObtainingAgentSessions { get; private set; }
    public State DebtCalculating { get; private set; }
    public State DebtRecording { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<PostpaymentDebtRecordingInitiated> StartSaga { get; private set; }
    public Event<SuccessfullyAgentSessionsObtained> SessionsObtained { get; private set; }
    public Event<UnsuccessfullyAgentSessionsObtained> ObtainingSessionsFailed { get; private set; }

    public Event<PostpaymentDebtCalculated> DebtCalculated { get; private set; }
    // TODO: Нужна ли обработка неудачного случая?

    public Event<SuccessfullyWrittenOffMessage> SuccessfullyDebtRecorded { get; private set; }
    public Event<UnsuccessfullyWrittenOffMessage> UnsuccessfullyDebtRecorded { get; private set; }

    public PostpaymentDebtRecordingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartSaga, x =>
        {
            x.CorrelateById(x => x.Message.OperationId);
            x.InsertOnInitial = true;
        });
        Event(() => SessionsObtained, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => ObtainingSessionsFailed, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => DebtCalculated, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => SuccessfullyDebtRecorded, x => x.CorrelateById(x => x.Message.OperationId));
        Event(() => UnsuccessfullyDebtRecorded, x => x.CorrelateById(x => x.Message.OperationId));

        Initially(When(StartSaga).Then(context =>
        {
            context.Saga.SubscriptionId = new SubscriptionId(context.Message.SubscriptionId);
            context.Saga.UserId = new UserId(context.Message.UserId);
            context.Saga.StartDateTime = context.Message.StartDateTime;
            context.Saga.EndDateTime = context.Message.EndDateTime;
        }).TransitionTo(ObtainingAgentSessions)
        .Publish(context => new AgentSessionsRequested(
            context.Saga.CorrelationId,
            context.Saga.UserId,
            context.Saga.StartDateTime,
            context.Saga.EndDateTime)
        ));

        During(ObtainingAgentSessions, When(SessionsObtained)
            .Publish(context => new SessionsObtainedEventMessage(
                new OperationId(context.Saga.CorrelationId),
                context.Saga.SubscriptionId,
                context.Saga.UserId,
                context.Message.AgentSessions
                ))
            .TransitionTo(DebtCalculating),
            When(ObtainingSessionsFailed)
            .IfElse(context => context.Saga.Retries < 3,
                x => x.Then(context => context.Saga.Retries++)
                .Publish(context => new AgentSessionsRequested(
                    context.Saga.CorrelationId,
                    context.Saga.UserId,
                    context.Saga.StartDateTime,
                    context.Saga.EndDateTime)
                ).TransitionTo(ObtainingAgentSessions),
                x => x.Publish(context => new ObtainingAgentSessionsRequiresAttention(
                        context.Saga.CorrelationId,
                        context.Saga.CreatedAt,
                        Reason: "Agent sessions obtaining failed 3 times")
                ).TransitionTo(Failed)
                .Finalize()
            )
        );

        During(DebtCalculating, When(DebtCalculated)
            .Then(context => context.Saga.Amount = context.Message.Amount)
            .Publish(context => new WriteOffFundsCommand(
                new OperationId(context.Saga.CorrelationId),
                context.Saga.Amount,
                IsForced: true
            )).TransitionTo(DebtRecording)
        );

        During(DebtRecording, When(SuccessfullyDebtRecorded)
            .Publish(context => new DebtRecordedEventMessage(
                context.Saga.SubscriptionId,
                context.Saga.UserId
            )).TransitionTo(Completed)
            .Finalize(), When(UnsuccessfullyDebtRecorded)
            .Publish(context => new InitiateDebtRecordingCommand(
                new OperationId(Guid.NewGuid()),
                context.Saga.UserId,
                context.Saga.Amount,
                new OperationId(context.Saga.CorrelationId)
            )).TransitionTo(Failed)
            .Finalize()
        );
    }
}
