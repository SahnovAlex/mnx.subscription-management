using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MNX.SubscriptionManagement.Application.UseCases.Tariff.Queries;
using MNX.SubscriptionManagement.Domain.Core;

namespace MNX.SubscriptionManagement.Service.Controllers;

/// <summary>
/// Контроллер для работы с тарифными планами.
/// </summary>
[Route("api/tariffPlans")]
[ApiController]
[Authorize]
public class TariffPlanController : ControllerBase
{
    private readonly IMediator _mediator;

    public TariffPlanController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Получить все тарифные планы.
    /// </summary>
    /// <returns> Тарифные планы. </returns>
    /// <response code="200"> Успешно. </response>
    /// <response code="401"> Пользователь не авторизирован. </response>
    [HttpGet]
    [ProducesResponseType(typeof(IAsyncEnumerable<TariffPlan>), 200)]
    public Task<IEnumerable<TariffPlan>> GetAll()
    {
        var result = _mediator.Send(new GetTariffPlansQuery());
        return result;
    }
}
