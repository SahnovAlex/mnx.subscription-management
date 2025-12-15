using EasyNetQ.AutoSubscribe;
using MediatR;
using MNX.SubscriptionManagement.Application.Events;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Service.Consumers;

// TODO: Возможно подпишется не на ту очередь
public sealed class DeleteSubscriptionCommandConsumer : IConsumeAsync<DeleteSubscriptionCommand>
{
    private readonly IMediator _mediator;

    public DeleteSubscriptionCommandConsumer(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task ConsumeAsync(DeleteSubscriptionCommand message, CancellationToken cancellationToken = default)
    {
        var @event = new DeleteSubscriptionEvent(new SubscriptionId(message.SubscriptionId), new UserId(message.UserId));
        return _mediator.Publish(@event, cancellationToken);
    }
}
