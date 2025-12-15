using EasyNetQ.AutoSubscribe;
using MediatR;
using MNX.SubscriptionManagement.Application.Events;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Service.Consumers;

public sealed class ScheduleSubscriptionRenewalCommandConsumer : IConsumeAsync<ScheduleSubscriptionRenewalCommand>
{
    public readonly IMediator _mediator;

    public ScheduleSubscriptionRenewalCommandConsumer(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task ConsumeAsync(ScheduleSubscriptionRenewalCommand message, CancellationToken cancellationToken = default)
    {
        var @event = new ScheduleSubscriptionRenewalEvent(new SubscriptionId(message.SubscriptionId), new UserId(message.UserId));
        return _mediator.Publish(@event, cancellationToken);
    }
}
