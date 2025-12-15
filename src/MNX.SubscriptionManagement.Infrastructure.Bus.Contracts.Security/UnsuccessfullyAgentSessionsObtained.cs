namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts.Security;

/// <summary>
/// Шаблон сообщения о неуспешном получении сессий
/// агента из сервиса безопасности.
/// </summary>
/// <remarks>
/// Данное сообщение должно быть реализовано в сервисе SecurityManagement.
/// </remarks>
/// <param name="OperationId"> Идентификатор операции. </param>
/// <param name="UserId"> Идентификатор пользователя. </param>
/// <param name="Reason"> Причина провала операции. </param>
public sealed record UnsuccessfullyAgentSessionsObtained(
    Guid OperationId,
    Guid UserId,
    string Reason
);
