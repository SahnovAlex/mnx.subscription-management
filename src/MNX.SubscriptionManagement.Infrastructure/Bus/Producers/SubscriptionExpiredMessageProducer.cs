using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Producers;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Producers;

/// <summary>
/// Реализация <see cref="ISubscriptionExpiredMessageProducer"/>.
/// </summary>
public sealed class SubscriptionExpiredMessageProducer : ISubscriptionExpiredMessageProducer
{
    private const string EXCHANGE_NAME =
        "MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.SubscriptionExpiredMessage, MNX.SubscriptionManagement.Infrastructure.Bus.Contracts";

    private readonly IAdvancedBus _bus;
    private readonly ILogger<SubscriptionExpiredMessageProducer> _logger;

    ///
    public SubscriptionExpiredMessageProducer(IAdvancedBus bus, ILogger<SubscriptionExpiredMessageProducer> logger)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task Produce(UserId userId, OperationId operationId, SubscriptionId subscriptionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Публикация сообщения об истечении подписки в SecurityService с идентификатором операции {operationId}", operationId.Value);
        var message = new SubscriptionExpiredMessage(OperationId: operationId, UserId: userId, SubscriptionId: subscriptionId);
        var wrappedMessage = new Message<SubscriptionExpiredMessage>(message);
        var exchange = _bus.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, cancellationToken: cancellationToken);
        return _bus.PublishAsync(exchange, string.Empty, false, wrappedMessage, cancellationToken);
    }
}
