using EasyNetQ.AutoSubscribe;
using MediatR;
using MNX.SubscriptionManagement.Application.UseCases.Subscription.Events;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.LicenseExtension.Events;

namespace MNX.SubscriptionManagement.Service.Consumers;

public sealed class LicenseExtensionSagaConsumer :
    IConsumeAsync<LicenseExtendedEvent>,
    IConsumeAsync<LicenseExtensionFailedEvent>
{
    private readonly IMediator _mediator;

    public LicenseExtensionSagaConsumer(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task ConsumeAsync(LicenseExtendedEvent message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ConsumeAsync(LicenseExtensionFailedEvent message, CancellationToken cancellationToken = default)
    {
        var subscriptionId = new SubscriptionId(message.SubscriptionId);
        var userId = new UserId(message.UserId);
        return _mediator.Publish(new DeleteSubscriptionEvent(subscriptionId, userId), cancellationToken);
    }
}
