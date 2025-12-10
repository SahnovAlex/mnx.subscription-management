using EasyNetQ.AutoSubscribe;
using MediatR;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Application.UseCases.SagaCommands;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Consumers;

/// <summary>
/// Потребитель сообщений <see cref="SubscriptionExpiredMessage"/>.
/// </summary>
public class SubscriptionExpiredMessageConsumer :
    IConsumeAsync<SubscriptionExpiredMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubscriptionExpiredMessageConsumer> _logger;

    public SubscriptionExpiredMessageConsumer(IMediator mediator,
                                              ILogger<SubscriptionExpiredMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить сообщение об истечении подписки.
    /// </summary>
    /// <param name="message"> Сообщение. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывает исключение при некорректном результате обработки сообщения.
    /// </exception>
    public async Task ConsumeAsync(SubscriptionExpiredMessage message, CancellationToken cancellationToken = default)
    {
        var operationIdValue = message.OperationId.ToString();
        _logger.LogInformation("Получено сообщение об истечении срока действия подписки с идентификатором операции - {operationId}",
                               operationIdValue);

        var command = new ProcessSubscriptionExpiredCommand(new OperationId(message.OperationId),
                                                            new UserId(message.UserId),
                                                            new SubscriptionId(message.SubscriptionId));

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var errors = string.Join(", ", result.Errors!);
            _logger.LogError("Обработка операции истечения подписки завершилась неудачей - {errors}", errors);
            throw new InvalidOperationException(
                $"The processing of license expiration operation failed. Operation id - {operationIdValue}");
        }
    }
}
