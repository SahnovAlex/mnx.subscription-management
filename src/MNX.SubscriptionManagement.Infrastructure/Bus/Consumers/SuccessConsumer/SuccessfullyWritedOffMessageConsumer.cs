using EasyNetQ.AutoSubscribe;
using MediatR;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Application.UseCases.SagaCommands.SuccessfullyResultCommands;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Consumers.SuccessConsumer;

/// <summary>
/// Потребитель сообщения <see cref="SuccessfullyWrittenOffMessage"/>.
/// </summary>
public class SuccessfullyWritedOffMessageConsumer : IConsumeAsync<SuccessfullyWrittenOffMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SuccessfullyWritedOffMessageConsumer> _logger;

    public SuccessfullyWritedOffMessageConsumer(IMediator mediator, ILogger<SuccessfullyWritedOffMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить сообщение об успешном снятии средств.
    /// </summary>
    /// <param name="message"> Сообщение. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <exception cref="InvalidOperationException">
    /// Возникает при некорректном результате обработки сообщения.
    /// </exception>
    public async Task ConsumeAsync(SuccessfullyWrittenOffMessage message, CancellationToken cancellationToken = default)
    {
        var operationIdValue = message.OperationId.ToString();
        _logger.LogInformation("Получено сообщение об успешном списании средств. Идентификатор операции - {operationId}",
                               operationIdValue);

        var operationId = new OperationId(message.OperationId);
        var command = new ProcessSuccessfullyWritingOffCommand(operationId);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var errors = string.Join(", ", result.Errors!);
            _logger.LogError("Обработка сообщения {messageName} завершилась неудачей - {errors}", nameof(SuccessfullyWrittenOffMessage), errors);
            throw new InvalidOperationException(
                $"The commit of writing off funds operation failed. Operation id - {operationIdValue}");
        }
    }
}
