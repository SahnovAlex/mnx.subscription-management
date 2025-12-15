using MediatR;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.Events;

public sealed record DeleteSubscriptionEvent(
    SubscriptionId SubscriptionId,
    UserId UserId
) : INotification;

public sealed class DeleteSubscriptionEventHandler : INotificationHandler<DeleteSubscriptionEvent>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPaymentScheduler _paymentScheduler;
    private readonly ILogger<DeleteSubscriptionEventHandler> _logger;

    public DeleteSubscriptionEventHandler(ISubscriptionRepository subscriptionRepository,
                                          IPaymentScheduler paymentScheduler,
                                          ILogger<DeleteSubscriptionEventHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository ??
            throw new ArgumentNullException(nameof(subscriptionRepository));
        _paymentScheduler = paymentScheduler ??
            throw new ArgumentNullException(nameof(paymentScheduler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(DeleteSubscriptionEvent notification, CancellationToken cancellationToken)
    {
        var currentSubscription = await _subscriptionRepository.GetById(
            notification.SubscriptionId, notification.UserId, cancellationToken);
        if (currentSubscription is null)
        {
            _logger.LogWarning("Подписка с идентификатором {subscriptionId} не найдена", notification.SubscriptionId.ToString());
            return;
        }

        await _paymentScheduler.DeleteSchedule(notification.SubscriptionId, cancellationToken);
        await _subscriptionRepository.Delete(notification.SubscriptionId, cancellationToken);
    }
}
