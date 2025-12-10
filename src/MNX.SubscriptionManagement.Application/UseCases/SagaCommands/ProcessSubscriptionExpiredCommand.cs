using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.CommandValidation;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.SagaCommands;

/// <summary>
/// Команда обработки истечения подписки.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="SubscriptionId"> Идентификатор подписки. </param>
public sealed record ProcessSubscriptionExpiredCommand(OperationId OperationId, UserId UserId, SubscriptionId SubscriptionId)
    : IValidatableCommand<Unit>;

/// <summary>
/// Обработчик <see cref="ProcessSubscriptionExpiredCommand"/>.
/// </summary>
public class ProcessSubscriptionExpiredCommandHandler : IRequestHandler<ProcessSubscriptionExpiredCommand, Result<Unit>>
{
    private readonly ILogger<ProcessSubscriptionExpiredCommandHandler> _logger;
    private readonly IPaymentScheduler _paymentScheduler;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISagaStatusesRepository _sagaStatusesRepository;
    private readonly ISubscriptionWithSagaRepository _subscriptionWithSagaRepository;

    public ProcessSubscriptionExpiredCommandHandler(ILogger<ProcessSubscriptionExpiredCommandHandler> logger,
                                                    IPaymentScheduler paymentScheduler,
                                                    ISubscriptionRepository subscriptionRepository,
                                                    ISagaStatusesRepository sagaStatusesRepository,
                                                    ISubscriptionWithSagaRepository subscriptionWithSagaRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _paymentScheduler = paymentScheduler ?? throw new ArgumentNullException(nameof(paymentScheduler));

        _subscriptionRepository = subscriptionRepository ??
            throw new ArgumentNullException(nameof(subscriptionRepository));

        _sagaStatusesRepository = sagaStatusesRepository ??
            throw new ArgumentNullException(nameof(sagaStatusesRepository));

        _subscriptionWithSagaRepository = subscriptionWithSagaRepository ??
            throw new ArgumentNullException(nameof(subscriptionWithSagaRepository));
    }

    public async Task<Result<Unit>> Handle(ProcessSubscriptionExpiredCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка операции отмены подписки с идентификатором {subscriptionId}. Идентификатор операции - {operationId}",
                               request.SubscriptionId.ToString(),
                               request.OperationId.ToString());

        var sagaOperation = await _sagaStatusesRepository.GetById(request.OperationId, cancellationToken);

        if (sagaOperation is null)
        {
            _logger.LogWarning("Операция с идентификатором {operationId} не найдена",
                               request.OperationId.ToString());

            return Result<Unit>.Empty();
        }

        if (sagaOperation.Status is not OperationStatus.SubscriptionExpired)
        {
            if (sagaOperation.Status is not OperationStatus.Success)
            {
                _logger.LogWarning("Операция с идентификатором {operationId} имеет некорректный статус - {operationStatus}. Ожидался статус - {expectedOperationStatus}",
                                   request.OperationId.ToString(),
                                   sagaOperation.Status,
                                   OperationStatus.SubscriptionExpired);
            }
            
            return Result<Unit>.Empty();
        }

        var subscription = await _subscriptionRepository.GetById(request.SubscriptionId, request.UserId, cancellationToken);
        if (subscription is null)
        {
            _logger.LogWarning("Подписка с идентификатором {subscriptionId} не найдена",
                               request.SubscriptionId.ToString());

            return Result<Unit>.Empty();
        }

        await _paymentScheduler.DeleteSchedule(request.SubscriptionId, cancellationToken);

        subscription.Deactivate();
        sagaOperation.Status = OperationStatus.Success;
        await _subscriptionWithSagaRepository.Update(sagaOperation, subscription, cancellationToken);

        _logger.LogInformation("Подписка с идентификатором {subscriptionId} была удалена по факту истечения",
                               request.SubscriptionId.ToString());

        return Result<Unit>.Empty();
    }
}
