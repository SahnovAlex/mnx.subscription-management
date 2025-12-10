using MNX.SubscriptionManagement.Domain.Core.ValueObjects;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace MNX.SubscriptionManagement.Infrastructure.External;

/// <summary>
/// Реализация <see cref="IExternalPaymentRepository"/>.
/// </summary>
public class ExternalPaymentRepository : IExternalPaymentRepository
{
    private readonly HttpClient _httpClient;
    
    public ExternalPaymentRepository(HttpClient httpClient)
    {
        _httpClient = httpClient ??
            throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc/>
    public async Task<bool> TryFreeze(OperationId operationId,
                                      UserId userId,
                                      float amount,
                                      CancellationToken cancellationToken = default)
    {
        var userIdValue = userId.Value.ToString();
        var body = new
        {
            amount = amount.ToString(CultureInfo.InvariantCulture),
            isForced = false,
            operationId = operationId.Value.ToString(),
            productId = "subscription"
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"api/users/{userIdValue}/balance/hold",
                                                    content,
                                                    cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ProcessBadRequest(response);
        }
        return true;
    }

    private static Task<bool> ProcessBadRequest(HttpResponseMessage response)
    {
        var statusCode = (int)response.StatusCode;
        if (statusCode >= 400 && statusCode < 500)
        {
            return Task.FromResult(false);
        }
        throw new HttpRequestException($"Something gone wrong while http requesting - {response.ReasonPhrase}");
    }
}