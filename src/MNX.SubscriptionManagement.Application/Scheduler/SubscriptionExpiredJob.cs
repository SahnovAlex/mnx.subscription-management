using MassTransit;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using Quartz;

namespace MNX.SubscriptionManagement.Application.Scheduler;

/// <summary>
/// Задание на истечение срока действия подписки.
/// </summary>
public class SubscriptionExpiredJob : IJob
{
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Ключ задачи.
    /// </summary>
    public static JobKey Key { get; } = new JobKey(nameof(SubscriptionExpiredJob));

    public SubscriptionExpiredJob(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint ??
            throw new ArgumentNullException(nameof(publishEndpoint));
    }

    /// <inheritdoc/>
    public async Task Execute(IJobExecutionContext context)
    {
        var subscriptionIdValue = context.JobDetail.JobDataMap.GetGuidValue("subscriptionId");
        var subscriptionId = new SubscriptionId(subscriptionIdValue);

        var userIdValue = context.JobDetail.JobDataMap.GetGuidValue("userId");
        var userId = new UserId(userIdValue);

        var operationId = new OperationId();
        
        // TODO: Реализация обработки окончания срока действия подписки.
        //await _publishEndpoint.Publish(new SubscriptionExpired(operationId, subscriptionId, userId));
    }
}
