using MediatR;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.Subscription.Events;

public sealed record AddSchedulerEvent(SubscriptionId SubscriptionId, UserId UserId) : INotification;

public sealed class AddSchedulerEventEventHandler : INotificationHandler<AddSchedulerEvent>
{
    private readonly IPaymentScheduler _paymentScheduler;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILogger<AddSchedulerEventEventHandler> _logger;

    public AddSchedulerEventEventHandler(IPaymentScheduler paymentScheduler,
                                         ISubscriptionRepository subscriptionRepository,
                                         ILogger<AddSchedulerEventEventHandler> logger)
    {
        _paymentScheduler = paymentScheduler ??
            throw new ArgumentNullException(nameof(paymentScheduler));
        _subscriptionRepository = subscriptionRepository ??
            throw new ArgumentNullException(nameof(subscriptionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(AddSchedulerEvent notification, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetById(
            notification.SubscriptionId, notification.UserId, cancellationToken);
        if (subscription is null)
        {
            _logger.LogWarning("Подписка с идентификатором {subscriptionId} не найдена", notification.SubscriptionId.ToString());
            return;
        }

        if (await _paymentScheduler.Exists(subscription.Id, cancellationToken))
        {
            _logger.LogWarning("Планировщик подписки с идентификатором {subscriptionId} уже существует", notification.SubscriptionId.ToString());
            return;
        }
        await _paymentScheduler.AddSchedule(subscription.UserId, subscription.Id, cancellationToken);
    }
}