using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Application.UseCases.SagaCommands.RollbackCommands;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.SagaCommands.UnsuccessfullyResultCommands;

/// <summary>
/// Команда обработки неудачного продления лицензии пользователя.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="FailureReason"> Описание причины неудачи операции. </param>
public sealed record ProcessUnsuccessfullyLicenseExtendingCommand(OperationId OperationId,
                                                                  UserId UserId,
                                                                  string FailureReason)
    : IRequest<Result<Unit>>;

/// <summary>
/// Обработчик команды <see cref="ProcessUnsuccessfullyLicenseExtendingCommand"/>.
/// </summary>
public class ProcessUnsuccessfullyLicenseExtendingCommandHandler
    : IRequestHandler<ProcessUnsuccessfullyLicenseExtendingCommand, Result<Unit>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessUnsuccessfullyLicenseExtendingCommandHandler> _logger;
    private readonly ISagaStatusesRepository _sagaStatusesRepository;

    public ProcessUnsuccessfullyLicenseExtendingCommandHandler(IMediator mediator,
                                                               ILogger<ProcessUnsuccessfullyLicenseExtendingCommandHandler> logger,
                                                               ISagaStatusesRepository sagaStatusesRepository)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sagaStatusesRepository = sagaStatusesRepository ?? throw new ArgumentNullException(nameof(sagaStatusesRepository));
    }

    public async Task<Result<Unit>> Handle(ProcessUnsuccessfullyLicenseExtendingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка неудачного продления лицензии пользователя. Идентификатор операции - {operationId}",
                               request.OperationId.ToString());

        var sagaOperation = await _sagaStatusesRepository.GetById(request.OperationId, cancellationToken);
        if (sagaOperation is null)
        {
            _logger.LogWarning("Операция с идентификатором {operationId} не найдена", request.OperationId.ToString());
            return Result<Unit>.Empty();
        }

        if (sagaOperation.Status is not OperationStatus.LicenseExtending)
        {
            if (sagaOperation.Status is not OperationStatus.Fail)
            {
                _logger.LogWarning("Операция с идентификатором {operationId} имеет статус {currentOperationStatus}. Ожидался статус {expectedOperationStatus}",
                               request.OperationId.ToString(),
                               sagaOperation.Status,
                               OperationStatus.LicenseExtending);
            }

            return Result<Unit>.Empty();
        }

        await _mediator.Send(new RollbackSubscriptionCreationCommand(request.UserId), cancellationToken);
        sagaOperation.Status = OperationStatus.Fail;
        await _sagaStatusesRepository.Update(sagaOperation, cancellationToken);

        _logger.LogInformation("Обработка неудачного продления лицензии завершена");
        return Result<Unit>.Invalid($"The license extending operation with id {request.OperationId.ToString()} failed. Reason - {request.FailureReason}");
    }
}