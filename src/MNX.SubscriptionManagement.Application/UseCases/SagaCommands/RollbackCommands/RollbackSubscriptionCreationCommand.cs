using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.SagaCommands.RollbackCommands;

/// <summary>
/// Команда отката создания подписки.
/// </summary>
/// <param name="UserId"> Идентификатор пользователя. </param>
public sealed record RollbackSubscriptionCreationCommand(UserId UserId)
    : IRequest<Result<Unit>>;

/// <summary>
/// Обработчик команды <see cref="RollbackSubscriptionCreationCommand"/>.
/// </summary>
public class RollbackSubscriptionCreationCommandHandler : IRequestHandler<RollbackSubscriptionCreationCommand, Result<Unit>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPaymentScheduler _paymentScheduler;
    private readonly ILogger<RollbackSubscriptionCreationCommandHandler> _logger;

    public RollbackSubscriptionCreationCommandHandler(ISubscriptionRepository subscriptionRepository,
                                                      IPaymentScheduler paymentScheduler,
                                                      ILogger<RollbackSubscriptionCreationCommandHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _paymentScheduler = paymentScheduler ?? throw new ArgumentNullException(nameof(paymentScheduler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Unit>> Handle(RollbackSubscriptionCreationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Операция отмены создания подписки пользователя с идентификатором {userId}",
                               request.UserId.Value.ToString());

        var currentSubscription = await _subscriptionRepository.GetCurrent(request.UserId, cancellationToken);
        if (currentSubscription is null)
        {
            _logger.LogInformation("Подписка пользователя с идентификатором {userId} уже отменена",
                                   request.UserId.Value.ToString());
            return Result<Unit>.Empty();
        }

        await _paymentScheduler.DeleteSchedule(currentSubscription.Id, cancellationToken);
        await _subscriptionRepository.Delete(currentSubscription.Id, cancellationToken);

        _logger.LogInformation("Подписка с идентификатором {subscriptionId} была удалена",
                               currentSubscription.Id.ToString());
        return Result<Unit>.Empty();
    }
}