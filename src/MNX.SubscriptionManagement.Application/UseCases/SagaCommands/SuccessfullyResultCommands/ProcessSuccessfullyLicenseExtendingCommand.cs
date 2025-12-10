using MediatR;
using Microsoft.Extensions.Logging;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Producers;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.SagaCommands.SuccessfullyResultCommands;

/// <summary>
/// Команда обработки успешного результата продления лицензии пользователя.
/// </summary>
/// <param name="OperationId"> Идентификатор операции. </param>
public sealed record ProcessSuccessfullyLicenseExtendingCommand(OperationId OperationId)
    : IRequest<Result<Unit>>;

/// <summary>
/// Обработчик команды <see cref="ProcessSuccessfullyLicenseExtendingCommand"/>.
/// </summary>
public class ProcessSuccessfullyLicenseExtendingCommandHandler : IRequestHandler<ProcessSuccessfullyLicenseExtendingCommand, Result<Unit>>
{
    private readonly ILogger<ProcessSuccessfullyLicenseExtendingCommandHandler> _logger;
    private readonly ISagaStatusesRepository _sagaStatusesRepository;
    private readonly ILicenseExtendedMessageProducer _licenseExtendedMessageProducer;

    public ProcessSuccessfullyLicenseExtendingCommandHandler(ILogger<ProcessSuccessfullyLicenseExtendingCommandHandler> logger,
                                                             ISagaStatusesRepository sagaStatusesRepository,
                                                             ILicenseExtendedMessageProducer licenseExtendedMessageProducer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _sagaStatusesRepository = sagaStatusesRepository ??
            throw new ArgumentNullException(nameof(sagaStatusesRepository));

        _licenseExtendedMessageProducer = licenseExtendedMessageProducer ??
            throw new ArgumentNullException(nameof(licenseExtendedMessageProducer));
    }

    public async Task<Result<Unit>> Handle(ProcessSuccessfullyLicenseExtendingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка успешного продления лицензии пользователя. Идентификатор операции - {operationId}",
                               request.OperationId.ToString());

        var sagaOperation = await _sagaStatusesRepository.GetById(request.OperationId, cancellationToken);
        if (sagaOperation is null)
        {
            _logger.LogWarning("Операция с идентификатором {operationId} не найдена", request.OperationId.ToString());
            return Result<Unit>.Empty();
        }

        if (sagaOperation.Status is not OperationStatus.LicenseExtending)
        {
            if (sagaOperation.Status is not OperationStatus.LicenseExtended)
            {
                _logger.LogWarning("Операция с идентификатором {operationId} имеет статус {currentOperationStatus}. Ожидался статус {expectedOperationStatus}",
                                   request.OperationId.ToString(),
                                   sagaOperation.Status,
                                   OperationStatus.LicenseExtending);
            }

            return Result<Unit>.Empty();
        }

        await _licenseExtendedMessageProducer.Produce(request.OperationId, true);

        sagaOperation.Status = OperationStatus.LicenseExtended;
        await _sagaStatusesRepository.Update(sagaOperation, cancellationToken);

        _logger.LogInformation("Операция продления лицензии завершена успешно");
        return Result<Unit>.Empty();
    }
}