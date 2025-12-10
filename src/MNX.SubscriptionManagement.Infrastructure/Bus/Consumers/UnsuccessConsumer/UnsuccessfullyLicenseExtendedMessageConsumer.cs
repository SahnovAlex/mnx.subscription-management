using EasyNetQ.AutoSubscribe;
using MediatR;
using Microsoft.Extensions.Logging;
using MNX.SecurityManagement.Licensing.Contracts;
using MNX.SubscriptionManagement.Application.UseCases.SagaCommands.UnsuccessfullyResultCommands;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Consumers.UnsuccessConsumer;

/// <summary>
/// Потребитель сообщений типа <see cref="UnsuccessfullyLicenseExtendedMessage"/>.
/// </summary>
public class UnsuccessfullyLicenseExtendedMessageConsumer : IConsumeAsync<UnsuccessfullyLicenseExtendedMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UnsuccessfullyLicenseExtendedMessageConsumer> _logger;

    public UnsuccessfullyLicenseExtendedMessageConsumer(IMediator mediator, ILogger<UnsuccessfullyLicenseExtendedMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить сообщение о неудачном продлении лицензии.
    /// </summary>
    /// <param name="message"> Сообщение. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <exception cref="InvalidOperationException">
    /// Должно быть выброшено, так как сообщение принимает неудачный результат.
    /// </exception>
    public async Task ConsumeAsync(UnsuccessfullyLicenseExtendedMessage message, CancellationToken cancellationToken = default)
    {
        var operationIdValue = message.OperationId.ToString();
        _logger.LogInformation("Получено сообщение о неудачном продлении лицензии. Идентификатор операции - {operationId}",
                               operationIdValue);

        var operationId = new OperationId(message.OperationId);
        var userId = new UserId(message.UserId);
        var command = new ProcessUnsuccessfullyLicenseExtendingCommand(operationId, userId, "");
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var errors = string.Join(", ", result.Errors!);
            _logger.LogError("Получено сообщение {messageName} по причине - {errors}", nameof(UnsuccessfullyLicenseExtendedMessage), errors);
            throw new InvalidOperationException(
                    $"The commit of license extending operation failed. Operation id - {operationIdValue}");
        }
    }
}
