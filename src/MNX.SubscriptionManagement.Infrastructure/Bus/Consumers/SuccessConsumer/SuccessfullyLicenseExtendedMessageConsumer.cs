using EasyNetQ.AutoSubscribe;
using MediatR;
using Microsoft.Extensions.Logging;
using MNX.SecurityManagement.Licensing.Contracts;
using MNX.SubscriptionManagement.Application.UseCases.SagaCommands.SuccessfullyResultCommands;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Consumers.SuccessConsumer;

/// <summary>
/// Потребитель сообщений типа <see cref="SuccessfullyLicenseExtendedMessage"/>.
/// </summary>
public class SuccessfullyLicenseExtendedMessageConsumer : IConsumeAsync<SuccessfullyLicenseExtendedMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SuccessfullyLicenseExtendedMessageConsumer> _logger;

    public SuccessfullyLicenseExtendedMessageConsumer(IMediator mediator, ILogger<SuccessfullyLicenseExtendedMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить сообщение об успешном продлении лицензии.
    /// </summary>
    /// <param name="message"> Сообщение. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <exception cref="InvalidOperationException">
    /// Возникает при некорректном результате обработки сообщения.
    /// </exception>
    public async Task ConsumeAsync(SuccessfullyLicenseExtendedMessage message, CancellationToken cancellationToken = default)
    {
        var operationIdValue = message.OperationId.ToString();
        _logger.LogInformation("Получено сообщение об успешном продлении лицензии. Идентификатор операции - {operationId}",
                               operationIdValue);

        var operationId = new OperationId(message.OperationId);
        var command = new ProcessSuccessfullyLicenseExtendingCommand(operationId);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var errors = string.Join(", ", result.Errors!);
            _logger.LogError("Обработка сообщения {messageName} завершилась неудачей - {errors}", nameof(SuccessfullyLicenseExtendedMessage), errors);
            throw new InvalidOperationException(
                $"The commit of license extending operation failed. Operation id - {operationIdValue}");
        }
    }
}
