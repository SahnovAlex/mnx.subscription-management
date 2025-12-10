using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.SagaCommands.UnsuccessfullyResultCommands;

/// <summary>
/// Команда обработки неудачного списания средств.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="FailureReason"> Описание причины неудачи операции. </param>
public sealed record ProcessUnsuccessfullyWritingOffCommand(OperationId OperationId, string FailureReason) : IRequest<Result<Unit>>;

/// <summary>
/// Обработчик команды <see cref="ProcessUnsuccessfullyWritingOffCommand"/>.
/// </summary>
public class ProcessUnsuccessfullyWritingOffCommandHandler : IRequestHandler<ProcessUnsuccessfullyWritingOffCommand, Result<Unit>>
{
    private readonly ILogger<ProcessUnsuccessfullyWritingOffCommandHandler> _logger;
    private readonly ISagaStatusesRepository _sagaStatusesRepository;

    public ProcessUnsuccessfullyWritingOffCommandHandler(ILogger<ProcessUnsuccessfullyWritingOffCommandHandler> logger,
                                                         ISagaStatusesRepository sagaStatusesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sagaStatusesRepository = sagaStatusesRepository ??
            throw new ArgumentNullException(nameof(sagaStatusesRepository));
    }

    public async Task<Result<Unit>> Handle(ProcessUnsuccessfullyWritingOffCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка неудачного снятия средств с баланса пользователя. Идентификатор операции - {operationId}",
                               request.OperationId.ToString());

        var sagaOperation = await _sagaStatusesRepository.GetById(request.OperationId, cancellationToken);
        if (sagaOperation is null)
        {
            _logger.LogWarning("Операция с идентификатором {operationId} не найдена", request.OperationId.ToString());
            return Result<Unit>.Empty();
        }

        if (sagaOperation.Status is not OperationStatus.LicenseExtended)
        {
            if (sagaOperation.Status is not OperationStatus.Fail)
            {
                _logger.LogWarning("Операция с идентификатором {operationId} имеет статус {currentOperationStatus}. {expectedOperationStatus}",
                               request.OperationId.ToString(),
                               sagaOperation.Status,
                               OperationStatus.LicenseExtended);
            }

            return Result<Unit>.Empty();
        }

        sagaOperation.Status = OperationStatus.Fail;
        await _sagaStatusesRepository.Update(sagaOperation, cancellationToken);

        _logger.LogInformation("Обработка неудачного продления средств завершена");
        return Result<Unit>.Invalid($"The funds debit operation with id {request.OperationId.ToString()} failed. Reason - {request.FailureReason}");
    }
}