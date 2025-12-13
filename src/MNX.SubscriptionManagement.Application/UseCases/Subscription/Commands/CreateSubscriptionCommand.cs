using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.CommandValidation;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Producers;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;
using MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

namespace MNX.SubscriptionManagement.Application.UseCases.Subscription.Commands;

/// <summary>
/// Команда на создание подписки.
/// </summary>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="TariffId"> Идентификатор тарифного плана. </param>
public sealed record CreateSubscriptionCommand(UserId UserId, TariffId TariffId, CancellationToken cancellationToken = default)
    : IValidatableCommand<Guid>;


/// <summary>
/// Обработчик <see cref="CreateSubscriptionCommand"/>.
/// </summary>
public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Result<Guid>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionStrategyFactory _subscriptionStrategyFactory;
    private readonly ISubscriptionCreatedMessageProducer _subscriptionCreatedMessageProducer;
    private readonly ITariffPlanRepository _tariffPlanRepository;
    private readonly ISagaStatusesRepository _sagaStatusesRepository;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

    ///
    public CreateSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository,
                                            ISubscriptionStrategyFactory subscriptionStrategyFactory,
                                            ISubscriptionCreatedMessageProducer producer,
                                            ITariffPlanRepository tariffPlanRepository,
                                            ISagaStatusesRepository sagaStatusesRepository,
                                            ILogger<CreateSubscriptionCommandHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository ??
            throw new ArgumentNullException(nameof(subscriptionRepository));
        _subscriptionStrategyFactory = subscriptionStrategyFactory ??
            throw new ArgumentNullException(nameof(subscriptionStrategyFactory));
        _subscriptionCreatedMessageProducer = producer ??
            throw new ArgumentNullException(nameof(producer));
        _tariffPlanRepository = tariffPlanRepository ??
            throw new ArgumentNullException(nameof(tariffPlanRepository));
        _sagaStatusesRepository = sagaStatusesRepository ??
            throw new ArgumentNullException(nameof(sagaStatusesRepository));
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

    ///
    public async Task<Result<Guid>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Попытка получения текущей подписки пользователя с идентификатором {userId}",
                               request.UserId.ToString());

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

        var subscriptionStrategyFactoryActivator =
            _subscriptionStrategyFactory.GetStrategyFamily(tariffPlan.PaymentStrategy).Activator;

        var operation = await InitiateSagaOperation(cancellationToken);

        var activationResult = await subscriptionStrategyFactoryActivator.ProcessAsync(operation.Id,
                                                                                       request.UserId,
                                                                                       tariffPlan,
                                                                                       cancellationToken);
        if (!activationResult.Result || activationResult.Subscription is null)
        {
            var reason = "The subscription was not activated";
            operation.Status = OperationStatus.Fail;
            operation.Reason = reason;

            await _sagaStatusesRepository.Update(operation, cancellationToken);
            return Result<Guid>.Error(reason);
        }

        await _subscriptionCreatedMessageProducer.Produce(request.UserId,
                                                          operation.Id,
                                                          activationResult.Subscription.EndDateTime,
                                                          cancellationToken);

        return Result<Guid>.SuccessfullyCreated(activationResult.Subscription.Id);
    }

    private async Task<SagaOperation> InitiateSagaOperation(CancellationToken cancellationToken)
    {
        var operationId = new OperationId();

        _logger.LogInformation("Инициализация операции саги по созданию подписки. Идентификатор операции - {operationId}",
                               operationId.ToString());

        var operation = new SagaOperation()
        {
            Id = operationId,
            Status = OperationStatus.LicenseExtending
        };
        await _sagaStatusesRepository.Add(operation, cancellationToken);

        return operation;
    }
}