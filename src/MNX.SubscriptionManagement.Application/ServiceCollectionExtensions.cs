using Microsoft.Extensions.DependencyInjection;
using MNX.SubscriptionManagement.Application.Scheduler;
using MNX.SubscriptionManagement.Application.UseCases.Subscription.Commands;
using MNX.SubscriptionManagement.Domain.Interfaces;

namespace MNX.SubscriptionManagement.Application;

/// <summary>
/// Расширения для <see cref="IServiceCollection"/> для регистрации модуля логики приложения.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавить бизнес-логику приложения.
    /// </summary>
    /// <param name="services"> Коллекция сервисов. </param>
    /// <returns> Коллекция сервисов. </returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateSubscriptionCommand).Assembly));

        services.AddScoped<IPaymentScheduler, PaymentScheduler>();

        return services;
    }
}
