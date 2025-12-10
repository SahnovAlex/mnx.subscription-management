using MediatR;
using MNX.Application.UseCases.Results;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;

namespace MNX.SubscriptionManagement.Application.UseCases.Subscription.Queries;

using Subscription = Domain.Core.Subscription;

/// <summary>
/// Запрос на получение текущей подписки.
/// </summary>
/// <param name="UserId"> Идентификатор пользователя. </param>
public sealed record GetCurrentSubscriptionQuery(UserId UserId) : IRequest<Result<Subscription>>;


/// <summary>
/// Обработчик <see cref="GetCurrentSubscriptionQuery"/>.
/// </summary>
public class GetCurrentSubscriptionQueryHandler
    : IRequestHandler<GetCurrentSubscriptionQuery, Result<Subscription>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetCurrentSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository
            ?? throw new ArgumentNullException(nameof(subscriptionRepository));
    }

    public async Task<Result<Subscription>> Handle(GetCurrentSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetCurrent(request.UserId, cancellationToken);

        return subscription is null
            ? Result<Subscription>.Empty()
            : Result<Subscription>.Success(subscription);
    }
}
