using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Producers;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Producers;

/// <summary>
/// Реализация <see cref="ISubscriptionCreatedMessageProducer"/>.
/// </summary>
public sealed class SubscriptionCreatedMessageProducer : ISubscriptionCreatedMessageProducer
{
    private const string EXCHANGE_NAME =
        "MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.SubscriptionCreatedMessage, MNX.SubscriptionManagement.Infrastructure.Bus.Contracts";

    private readonly IAdvancedBus _bus;
    private readonly ILogger<SubscriptionCreatedMessageProducer> _logger;

    ///
    public SubscriptionCreatedMessageProducer(IAdvancedBus bus, ILogger<SubscriptionCreatedMessageProducer> logger)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task Produce(UserId userId, OperationId operationId, DateTimeOffset expirationDateTime, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Публикация сообщения о создании подписки в сервис security с идентификатором операции {operationId}", operationId.Value);
        var message = new SubscriptionCreatedMessage(UserId: userId, OperationId: operationId, ExpirationDateTime: expirationDateTime);
        var wrappedMessage = new Message<SubscriptionCreatedMessage>(message);
        var exchange = _bus.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, cancellationToken: cancellationToken);
        return _bus.PublishAsync(exchange, string.Empty, false, wrappedMessage, cancellationToken);
    }
}
