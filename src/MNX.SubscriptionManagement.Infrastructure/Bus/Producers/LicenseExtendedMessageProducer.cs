using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Producers;
using MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

namespace MNX.SubscriptionManagement.Infrastructure.Bus.Producers;

/// <summary>
/// Реализация <see cref="ILicenseExtendedMessageProducer"/>.
/// </summary>
public class LicenseExtendedMessageProducer : ILicenseExtendedMessageProducer
{
    // TODO: Определить имя очереди на стороне payment.
    private const string QUEUE_NAME =
        "MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.LicenseExtendedMessage, MNX.SubscriptionManagement.Infrastructure.Bus.Contracts_LicenseExtendedMessage, security";

    private readonly ILogger<LicenseExtendedMessageProducer> _logger;
    private readonly IAdvancedBus _bus;

    public LicenseExtendedMessageProducer(ILogger<LicenseExtendedMessageProducer> logger, IAdvancedBus bus)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    /// <inheritdoc/>
    public Task Produce(OperationId operationId, bool isForced, CancellationToken cancellationToken = default)
    {
        var operationIdValue = operationId.Value.ToString();
        _logger.LogInformation("Публикация сообщения о продлении лицензии пользователя. Идентификатор операции - {operationId}",
                               operationIdValue);

        var exchange = _bus.ExchangeDeclare(QUEUE_NAME, ExchangeType.Direct, cancellationToken: cancellationToken);
        var message = new LicenseExtendedMessage(OperationId: operationId, IsForced: isForced);
        var wrappedMessage = new Message<LicenseExtendedMessage>(message);
        return _bus.PublishAsync(exchange, string.Empty, false, wrappedMessage, cancellationToken);
    }
}
