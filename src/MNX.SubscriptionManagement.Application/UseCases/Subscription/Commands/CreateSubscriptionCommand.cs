using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.CommandValidation;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Application.SagaInitiation;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.Subscription.Commands;

public sealed record CreateSubscriptionCommand(UserId UserId, TariffId TariffId, CancellationToken CancellationToken = default)
    : IValidatableCommand<Guid>;

public sealed class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, Result<Guid>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITariffPlanRepository _tariffPlanRepository;
    private readonly IExternalPaymentRepository _externalPaymentRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateSubscriptionHandler(ISubscriptionRepository subscriptionRepository,
                                     ITariffPlanRepository tariffPlanRepository,
                                     IExternalPaymentRepository externalPaymentRepository,
                                     IPublishEndpoint publishEndpoint,
                                     ILogger<CreateSubscriptionHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository ??
            throw new ArgumentNullException(nameof(subscriptionRepository));
        _tariffPlanRepository = tariffPlanRepository ??
            throw new ArgumentNullException(nameof(tariffPlanRepository));
        _externalPaymentRepository = externalPaymentRepository ??
            throw new ArgumentNullException(nameof(externalPaymentRepository));
        _publishEndpoint = publishEndpoint ??
            throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<Result<Guid>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var operationId = new OperationId(Guid.NewGuid());

        var currentSubscription = await _subscriptionRepository.GetCurrent(request.UserId, cancellationToken);
        if (currentSubscription is not null)
        {
            return Result<Guid>.Conflict("The active subscription already exists");
        }

        var tariffPlan = await _tariffPlanRepository.GetById(request.TariffId, cancellationToken);
        if (tariffPlan is null)
        {
            var tariffId = request.TariffId.Value.ToString();
            return Result<Guid>.Invalid($"Tariff plan with id {tariffId} does not exists");
        }

        var freezeResult = await _externalPaymentRepository.TryFreeze(
            operationId, request.UserId, GetTariffPriceForFreezing(tariffPlan), cancellationToken);

        if (!freezeResult)
            return Result<Guid>.Invalid("Failed to subscribe due to wallet issues");

        var subscription = new Domain.Core.Subscription(request.UserId, request.TariffId);

        await _subscriptionRepository.Create(subscription, cancellationToken);

        if (tariffPlan.PaymentStrategy == PaymentStrategyType.Prepayment)
        {
            var command = new RenewalSubscriptionInitiated(
                operationId, request.UserId, subscription.EndDateTime, tariffPlan.Price);
            await _publishEndpoint.Publish(command, cancellationToken);
        }
        if (tariffPlan.PaymentStrategy == PaymentStrategyType.Postpayment)
        {
            var command = new LicenseExtensionInitiated(operationId, request.UserId, subscription.EndDateTime);
            await _publishEndpoint.Publish(command, cancellationToken);
        }

        return Result<Guid>.SuccessfullyCreated(subscription.Id);
    }

    private static float GetTariffPriceForFreezing(TariffPlan tariffPlan)
        => tariffPlan.PaymentStrategy == PaymentStrategyType.Prepayment ? tariffPlan.Price : 0.0F;
}