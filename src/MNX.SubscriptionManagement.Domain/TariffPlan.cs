using MNX.SubscriptionManagement.Domain.Core.Enums;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Core;

/// <summary>
/// Тарифный план.
/// </summary>
public class TariffPlan
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public TariffId Id { get; init; }

    /// <summary>
    /// Название.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Цена за тариф.
    /// </summary>
    public float Price { get; set; }

    /// <summary>
    /// Стратегия оплаты тарифа.
    /// </summary>
    public PaymentStrategyType PaymentStrategy { get; init; }
}
