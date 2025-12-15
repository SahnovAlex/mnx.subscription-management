using Microsoft.Extensions.Logging;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces;
using Quartz;

namespace MNX.SubscriptionManagement.Application.Service.Scheduler;

/// <summary>
/// Реализация <see cref="IPaymentScheduler"/>.
/// </summary>
public class PaymentScheduler : IPaymentScheduler
{
    private readonly ILogger<PaymentScheduler> _logger;

    private readonly ISchedulerFactory _schedulerFactory;

    private const string GROUP_ID = "subscription_management";

    public PaymentScheduler(ILogger<PaymentScheduler> logger, ISchedulerFactory schedulerFactory)
    {
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        _schedulerFactory = schedulerFactory
            ?? throw new ArgumentNullException(nameof(schedulerFactory));
    }

    /// <inheritdoc/>
    public async Task<bool> Exists(SubscriptionId subscriptionId, CancellationToken cancellationToken = default)
    {
        var triggerKey = new TriggerKey(subscriptionId.ToString(), GROUP_ID);
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        return await scheduler.CheckExists(triggerKey, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddSchedule(UserId userId, SubscriptionId subscriptionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var job = JobBuilder.Create<SubscriptionExpiredJob>()
                .WithIdentity(subscriptionId.ToString(), GROUP_ID)
                .UsingJobData("userId", userId.ToString())
                .UsingJobData("subscriptionId", subscriptionId.ToString())
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(subscriptionId.ToString(), GROUP_ID)
                .StartAt(DateBuilder.NextGivenMinuteDate(null, 0).AddMonths(1).AddSeconds(-1))
                .WithCalendarIntervalSchedule(x => x.WithIntervalInMonths(1))
                .Build();

            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error while trigger creating - {message}", ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task DeleteSchedule(SubscriptionId subscriptionId, CancellationToken cancellationToken = default)
    {
        var triggerKey = new TriggerKey(subscriptionId.ToString(), GROUP_ID);
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.UnscheduleJob(triggerKey, cancellationToken);
    }
}
