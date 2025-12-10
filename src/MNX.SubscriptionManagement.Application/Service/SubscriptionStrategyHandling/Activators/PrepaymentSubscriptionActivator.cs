using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;
using MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

namespace MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling.Activators;

/// <summary>
/// Реализация <see cref="ISubscriptionActivator"/> для стратегии предоплаты подписки.
/// </summary>
public class PrepaymentSubscriptionActivator : ISubscriptionActivator
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IExternalPaymentRepository _externalPaymentRepository;
    private readonly IPaymentScheduler _paymentScheduler;
    private readonly ILogger<PrepaymentSubscriptionActivator> _logger;

    public PrepaymentSubscriptionActivator(ISubscriptionRepository subscriptionRepository,
                                           IExternalPaymentRepository externalPaymentRepository,
                                           IPaymentScheduler paymentScheduler,
                                           ILogger<PrepaymentSubscriptionActivator> logger)
    {
        _subscriptionRepository = subscriptionRepository ??
            throw new ArgumentNullException(nameof(subscriptionRepository));

        _externalPaymentRepository = externalPaymentRepository ??
            throw new ArgumentNullException(nameof(externalPaymentRepository));

        _paymentScheduler = paymentScheduler ??
            throw new ArgumentNullException(nameof(paymentScheduler));

        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<(bool Result, Subscription? Subscription)> ProcessAsync(OperationId operationId,
                                                                              UserId userId,
                                                                              TariffPlan tariffPlan,
                                                                              CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Попытка заморозки суммы по тарифу {tariffName}",
                               tariffPlan.Name.ToString());

        var freezeResult = await _externalPaymentRepository.TryFreeze(operationId, userId, tariffPlan.Price, cancellationToken);
        if (!freezeResult)
        {
            _logger.LogWarning("Попытка заморозки на балансе пользователя с идентификатором {userId} завершилась неудачей",
                               userId.Value.ToString());

            return (false, null);
        }

        var subscription = await CreateSubscriptionWithScheduler(userId, tariffPlan.Id, cancellationToken);

        return (true, subscription);
    }

    private async Task<Subscription> CreateSubscriptionWithScheduler(UserId userId,
                                                                     TariffId tariffId,
                                                                     CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Создание подписки пользователя с идентификатором {userId}",
                               userId.Value.ToString());

        var subscription = new Subscription(userId, tariffId);
        await _subscriptionRepository.Create(subscription, cancellationToken);

        _logger.LogInformation("Создание планировщика для подписки с идентификатором {subscriptionId}",
                               subscription.Id.Value.ToString());

        await _paymentScheduler.AddSchedule(userId, subscription.Id, cancellationToken);

        _logger.LogInformation("Подписка с идентификатором {subscriptionId} создана и добавлен планировщик",
                               subscription.Id.Value.ToString());

        return subscription;
    }
}
