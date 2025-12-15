namespace MNX.SubscriptionManagement.Infrastructure.Bus.Contracts;

public sealed record PostpaymentDebtCalculated(
    Guid OperationId,
    Guid UserId,
    float Amount
);
