using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MNX.Application.UseCases.Results;
using MNX.SecurityManagement.Authorization;
using MNX.SubscriptionManagement.Application.UseCases.Subscription.Commands;
using MNX.SubscriptionManagement.Application.UseCases.Subscription.Queries;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Service.Controllers;

/// <summary>
/// Контроллер для работы с подписками.
/// </summary>
[Route("api/subscriptions")]
[ApiController]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthorizedClientAccessor _clientAccessor;

    public SubscriptionController(IMediator mediator, IAuthorizedClientAccessor clientAccessor)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _clientAccessor = clientAccessor ?? throw new ArgumentNullException(nameof(clientAccessor));
    }

    /// <summary>
    /// Получить текущую подписку.
    /// </summary>
    /// <returns> Результат получения подписки. </returns>
    /// <response code="200"> Подписка найдена. </response>
    /// <response code="204"> Подписки нет. </response>
    /// <response code="401"> Пользователь не авторизирован. </response>
    [HttpGet]
    [ProducesResponseType(typeof(Subscription), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetCurrent()
    {
        var userId = _clientAccessor.Id;
        var result = await _mediator.Send(new GetCurrentSubscriptionQuery(new UserId(userId)));
        return result.ToActionResult();
    }

    /// <summary>
    /// Оформить подписку.
    /// </summary>
    /// <param name="tariffPlanId"> Идентификатор тарифного плана. </param>
    /// <returns> Результат тарифного плана. </returns>
    /// <response code="201"> Успешно. </response>
    /// <response code="400"> Тарифный план не был найден. </response>
    /// <response code="409"> Подписка уже оформлена. </response>
    /// <response code="422"> Ошибка оформления подписки. </response>
    [HttpPost]
    [ProducesResponseType(typeof(Subscription), 201)]
    [ProducesResponseType(typeof(List<string>), 400)]
    [ProducesResponseType(typeof(List<string>), 409)]
    [ProducesResponseType(typeof(List<string>), 422)]
    public async Task<IActionResult> Create(Guid tariffPlanId)
    {
        if (tariffPlanId == Guid.Empty)
            return BadRequest("Invalid tariff plan identifier");

        var userId = _clientAccessor.Id;
        var result = await _mediator.Send(new CreateSubscriptionCommand(new UserId(userId), new TariffId(tariffPlanId)));
        return result.ToActionResult();
    }
}
