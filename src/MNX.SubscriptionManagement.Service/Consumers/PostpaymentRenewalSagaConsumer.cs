using EasyNetQ.AutoSubscribe;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal.Events;

namespace MNX.SubscriptionManagement.Service.Consumers;

public class PostpaymentRenewalSagaConsumer :
    IConsumeAsync<ObtainingAgentSessionsFailed>,
    IConsumeAsync<PostpaymentDebtRecordedEvent>,
    IConsumeAsync<PostpaymentDebtRecordingFailedEvent>,
    IConsumeAsync<SessionsObtainedEvent>
{
    public Task ConsumeAsync(ObtainingAgentSessionsFailed message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeAsync(PostpaymentDebtRecordedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeAsync(PostpaymentDebtRecordingFailedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeAsync(SessionsObtainedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
