using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Producers;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;
using Quartz;

namespace MNX.SubscriptionManagement.Application.Service.Scheduler;

/// <summary>
/// Задание на истечение срока действия подписки.
/// </summary>
public class SubscriptionExpiredJob : IJob
{
    private readonly ISubscriptionExpiredMessageProducer _producer;
    private readonly ISagaStatusesRepository _sagaStatusesRepository;

    /// <summary>
    /// Ключ задачи.
    /// </summary>
    public static JobKey Key { get; } = new JobKey(nameof(SubscriptionExpiredJob));

    public SubscriptionExpiredJob(ISubscriptionExpiredMessageProducer producer,
                                    ISagaStatusesRepository sagaStatusesRepository)
    {
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
        _sagaStatusesRepository = sagaStatusesRepository ??
            throw new ArgumentNullException(nameof(sagaStatusesRepository));
    }

    /// <inheritdoc/>
    public async Task Execute(IJobExecutionContext context)
    {
        var subscriptionIdValue = context.JobDetail.JobDataMap.GetGuidValue("subscriptionId");
        var subscriptionId = new SubscriptionId(subscriptionIdValue);

        var userIdValue = context.JobDetail.JobDataMap.GetGuidValue("userId");
        var userId = new UserId(userIdValue);

        var operation = new SagaOperation()
        {
            Id = new OperationId(),
            Status = OperationStatus.SubscriptionExpired
        };
        await _sagaStatusesRepository.Add(operation);

        await _producer.Produce(userId, operation.Id, subscriptionId);
    }
}
