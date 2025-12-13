using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий тарифных планов.
/// </summary>
public interface ITariffPlanRepository
{
    /// <summary>
    /// Получить все тарифные планы.
    /// </summary>
    /// <returns> Список тарифных планов. </returns>
    Task<IEnumerable<TariffPlan>> GetAll();

    /// <summary>
    /// Проверить существование тарифного плана.
    /// </summary>
    /// <param name="tariffId"> Идентификатор тарифа. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <returns> Признак существования тарифного плана. </returns>
    Task<bool> Exists(TariffId tariffId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить тарифный план по идентификатору.
    /// </summary>
    /// <param name="id"> Идентификатор тарифного плана. </param>
    /// <param name="cancellationToken"> Токен отмены. </param>
    /// <returns> Тарифный план. </returns>
    Task<TariffPlan?> GetById(TariffId id, CancellationToken cancellationToken = default);
}
