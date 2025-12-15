using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.CommandValidation;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Application.SagaInitiation;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.Subscription.Commands;

public sealed record CancelSubscriptionCommand(UserId UserId) : IValidatableCommand<Unit>;

public sealed class CancelSubscriptionCommandHandler
    : IRequestHandler<CancelSubscriptionCommand, Result<Unit>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPaymentScheduler _paymentScheduler;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CancelSubscriptionCommandHandler> _logger;

    public CancelSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository,
                                            IPaymentScheduler paymentScheduler,
                                            IPublishEndpoint publishEndpoint,
                                            ILogger<CancelSubscriptionCommandHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository ??
            throw new ArgumentNullException(nameof(subscriptionRepository));
        _paymentScheduler = paymentScheduler ??
            throw new ArgumentNullException(nameof(paymentScheduler));
        _publishEndpoint = publishEndpoint ??
            throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Unit>> Handle(CancelSubscriptionCommand request,
                                           CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetCurrent(request.UserId, cancellationToken);
        if (subscription is null)
        {
            _logger.LogWarning("Подписка пользователя с идентификатором {userId} не найдена",
                               request.UserId.ToString());
            return Result<Unit>.Invalid(
                $"Subscription of user with identifier {request.UserId.ToString()} wasn't found");
        }

        if (subscription.TariffPlan is null)
        {
            _logger.LogWarning("Тарифный план подписки с идентификатором {tariffId} не найдена",
                               subscription.TariffPlanId.ToString());
            return Result<Unit>.Invalid(
                $"Tariff plan with identifier {subscription.TariffPlanId.ToString()} wasn't found");
        }

        subscription.Deactivate();

        if (subscription.TariffPlan.PaymentStrategy == PaymentStrategyType.Postpayment)
        {
            subscription.InterruptSubscriptionPeriod();
            await _paymentScheduler.DeleteSchedule(subscription.Id, cancellationToken);
            var command = new PostpaymentDebtRecordingInitiated(new OperationId(),
                                                                subscription.Id,
                                                                subscription.UserId,
                                                                subscription.StartDateTime,
                                                                subscription.EndDateTime);
            await _publishEndpoint.Publish(command, cancellationToken);
        }

        await _subscriptionRepository.Update(subscription, cancellationToken);
        return Result<Unit>.Empty();
    }
}