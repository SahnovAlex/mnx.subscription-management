using EasyNetQ.AutoSubscribe;
using MediatR;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Application.UseCases.SagaCommands.UnsuccessfullyResultCommands;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Consumers.UnsuccessConsumer;

/// <summary>
/// Потребитель сообщений типа <see cref="UnsuccessfullyWrittenOffMessage"/>.
/// </summary>
public class UnsuccessfullyWrittenOffMessageConsumer : IConsumeAsync<UnsuccessfullyWrittenOffMessage>
{
    private readonly ILogger<UnsuccessfullyWrittenOffMessageConsumer> _logger;
    private readonly IMediator _mediator;

    public UnsuccessfullyWrittenOffMessageConsumer(ILogger<UnsuccessfullyWrittenOffMessageConsumer> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Получить сообщение о неудачном списании средств.
    /// </summary>
    /// <param name="message"> Сообщение. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <exception cref="InvalidOperationException">
    /// Должно быть выброшено, так как сообщение принимает неудачный результат.
    /// </exception>
    public async Task ConsumeAsync(UnsuccessfullyWrittenOffMessage message, CancellationToken cancellationToken = default)
    {
        var operationIdValue = message.OperationId.ToString();
        _logger.LogInformation("Получено сообщение о неудачном списании средств. Идентификатор операции - {operationId}",
                               operationIdValue);

        var operationId = new OperationId(message.OperationId);
        var command = new ProcessUnsuccessfullyWritingOffCommand(operationId, message.FailureReason);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var errors = string.Join(", ", result.Errors!);
            _logger.LogError("Получено сообщение {messageName} по причине - {errors}", nameof(UnsuccessfullyWrittenOffMessage), errors);
            throw new InvalidOperationException(
                    $"The commit of writing off funds operation failed. Operation id - {operationIdValue}");
        }
    }
}
