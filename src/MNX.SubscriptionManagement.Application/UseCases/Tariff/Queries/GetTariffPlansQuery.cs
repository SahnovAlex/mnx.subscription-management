using MediatR;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.Tariff.Queries;

/// <summary>
/// Запрос на получение тарифных планов.
/// </summary>
public sealed record GetTariffPlansQuery : IRequest<IEnumerable<TariffPlan>>;

/// <summary>
/// Обработчик <see cref="GetTariffPlansQuery"/>.
/// </summary>
public class GetTariffPlansQueryHandler : IRequestHandler<GetTariffPlansQuery, IEnumerable<TariffPlan>>
{
    private readonly ITariffPlanRepository _tariffRepository;

    ///
    public GetTariffPlansQueryHandler(ITariffPlanRepository tariffRepository)
    {
        _tariffRepository = tariffRepository ?? throw new ArgumentNullException(nameof(tariffRepository));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TariffPlan>> Handle(GetTariffPlansQuery request, CancellationToken cancellationToken)
    {
        return await _tariffRepository.GetAll();
    }
}
