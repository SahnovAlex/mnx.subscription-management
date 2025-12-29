using EasyNetQ.AutoSubscribe;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.DebtRecording.Events;

namespace MNX.SubscriptionManagement.Service.Consumers;

public class DebtRecordingSagaConsumer :
    IConsumeAsync<DebtRecordedEvent>,
    IConsumeAsync<DebtRecordingFailed>
{
    public Task ConsumeAsync(DebtRecordedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeAsync(DebtRecordingFailed message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
