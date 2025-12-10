using Microsoft.Extensions.DependencyInjection;
using MNX.SubscriptionManagement.Application.Service.Scheduler;
using MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling;
using MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling.Activators;
using MNX.SubscriptionManagement.Application.Service.SubscriptionStrategyHandling.Extenders;
using MNX.SubscriptionManagement.Application.UseCases.Subscription.Commands;
using MNX.SubscriptionManagement.Domain.Interfaces;
using MNX.SubscriptionManagement.Domain.Interfaces.SubscriptionService;

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

        services.AddSubscriptionStrategies();

        return services;
    }

    private static IServiceCollection AddSubscriptionStrategies(this IServiceCollection services)
    {
        services.AddScoped<PrepaymentSubscriptionActivator>();
        services.AddScoped<PrepaymentSubscriptionExtender>();
        services.AddScoped<ISubscriptionStrategyFamily, PrepaymentStrategyFamily>();
        services.AddScoped<ISubscriptionStrategyFactory, SubscriptionStrategyFactory>();

        return services;
    }
}
