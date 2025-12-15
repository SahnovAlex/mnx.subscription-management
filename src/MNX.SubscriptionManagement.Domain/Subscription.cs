using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Core;

/// <summary>
/// Подписка.
/// </summary>
public class Subscription
{
    private DateTimeOffset _endDateTime;
    private SubscriptionStatus _status;
    private bool _autoExtend;

    public Subscription(UserId userId, TariffId tariffPlanId, int validityPeriod = 1)
    {
        UserId = userId;
        StartDateTime = DateTimeOffset.UtcNow.Date;
        EndDateTime = StartDateTime.AddMonths(validityPeriod);
        SubscriptionPeriod = validityPeriod;
        Status = SubscriptionStatus.Active;
        AutoExtend = true;
        TariffPlanId = tariffPlanId;
    }

    /// <summary>
    /// Идентификатор подписки.
    /// </summary>
    public SubscriptionId Id { get; init; } = new SubscriptionId();

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public UserId UserId { get; init; }

    /// <summary>
    /// Дата и время активации.
    /// </summary>
    public DateTimeOffset StartDateTime { get; init; }

    /// <summary>
    /// Дата и время окончания действия подписки.
    /// </summary>
    public DateTimeOffset EndDateTime
    {
        get => _endDateTime;
        private set => _endDateTime = value;
    }

    /// <summary>
    /// Период действия подписки.
    /// </summary>
    public int SubscriptionPeriod { get; init; }

    /// <summary>
    /// Статус
    /// </summary>
    public SubscriptionStatus Status
    {
        get => _status;
        init => _status = value;
    }

    /// <summary>
    /// Признак автоматического продления.
    /// </summary>
    public bool AutoExtend
    {
        get => _autoExtend;
        init => _autoExtend = value;
    }

    /// <summary>
    /// Идентификатор тарифного плана.
    /// </summary>
    public TariffId TariffPlanId { get; init; }

    /// <summary>
    /// Тарифный план подписки.
    /// </summary>
    public TariffPlan? TariffPlan { get; init; }

    /// <summary>
    /// Деактивировать подписку.
    /// </summary>
    public void Deactivate()
    {
        _status = SubscriptionStatus.Cancelled;
        _autoExtend = false;
    }

    /// <summary>
    /// Прервать дату окончания действия подписки.
    /// </summary>
    public void InterruptSubscriptionPeriod()
    {
        // TODO: По идее, необходимо проверять, является ли тариф подписки постоплатным.
        EndDateTime = DateTimeOffset.UtcNow;
    }
}
