namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

/// <summary>
/// Команда продления периода планировщика. 
/// </summary>
/// <param name="SubscriptionId"> Идентификатор подписки. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
public sealed record ScheduleSubscriptionRenewalCommand(Guid SubscriptionId, Guid UserId);
