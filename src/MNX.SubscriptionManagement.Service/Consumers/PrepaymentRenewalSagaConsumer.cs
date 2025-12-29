using EasyNetQ.AutoSubscribe;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal.Events;

namespace MNX.SubscriptionManagement.Service.Consumers;

public class PrepaymentRenewalSagaConsumer :
    IConsumeAsync<PrepaymentDebtRecordedEvent>,
    IConsumeAsync<PrepaymentDebtRecordingFailedEvent>,
    IConsumeAsync<PrepaymentLicenseExtensionFailedEvent>
{
    public Task ConsumeAsync(PrepaymentDebtRecordedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeAsync(PrepaymentDebtRecordingFailedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeAsync(PrepaymentLicenseExtensionFailedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
