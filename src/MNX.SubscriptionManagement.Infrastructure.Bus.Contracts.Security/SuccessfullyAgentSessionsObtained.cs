namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Security;

/// <summary>
/// Шаблон для сообщения об успешном получении сессий агента
/// из сервиса безопасности.
/// </summary>
/// <remarks>
/// Данное сообщение должно быть реализовано в сервисе SecurityManagement.
/// </remarks>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="AgentSessions"> Список сессий агента. </param>
public sealed record SuccessfullyAgentSessionsObtained(
    Guid OperationId,
    Guid UserId,
    List<string> AgentSessions
);
