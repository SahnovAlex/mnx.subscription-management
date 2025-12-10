using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.SagaCommands.SuccessfullyResultCommands;

/// <summary>
/// Команда обработки успешного результата списания средств.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
public sealed record ProcessSuccessfullyWritingOffCommand(OperationId OperationId) : IRequest<Result<Unit>>;

/// <summary>
/// Обработчик команды <see cref="ProcessSuccessfullyWritingOffCommand"/>.
/// </summary>
public class ProcessSuccessfullyWritingOffCommandHandler : IRequestHandler<ProcessSuccessfullyWritingOffCommand, Result<Unit>>
{
    private readonly ILogger<ProcessSuccessfullyWritingOffCommandHandler> _logger;
    private readonly ISagaStatusesRepository _sagaStatusesRepository;

    public ProcessSuccessfullyWritingOffCommandHandler(ILogger<ProcessSuccessfullyWritingOffCommandHandler> logger,
                                                       ISagaStatusesRepository sagaStatusesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _sagaStatusesRepository = sagaStatusesRepository ??
            throw new ArgumentNullException(nameof(sagaStatusesRepository));
    }

    public async Task<Result<Unit>> Handle(ProcessSuccessfullyWritingOffCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка успешного снятия средств с баланса пользователя. Идентификатор операции - {operationId}",
                               request.OperationId.ToString());

        var sagaOperation = await _sagaStatusesRepository.GetById(request.OperationId, cancellationToken);
        if (sagaOperation is null)
        {
            _logger.LogWarning("Операция с идентификатором {operationId} не найдена", request.OperationId.ToString());
            return Result<Unit>.Empty();
        }

        if (sagaOperation.Status is not OperationStatus.LicenseExtended)
        {
            if (sagaOperation.Status is not OperationStatus.Success)
            {
                _logger.LogWarning("Операция с идентификатором {operationId} имеет статус {currentOperationStatus}. Ожидался статус {expectedOperationStatus}",
                               request.OperationId.ToString(),
                               sagaOperation.Status,
                               OperationStatus.LicenseExtended);
            }

            return Result<Unit>.Empty();
        }

        sagaOperation.Status = OperationStatus.Success;
        await _sagaStatusesRepository.Update(sagaOperation, cancellationToken);

        _logger.LogInformation("Операция снятия средств завершена успешно. Завершение распределенной транзакции с идентификатором {operationId}",
                               request.OperationId.ToString());

        return Result<Unit>.Empty();
    }
}