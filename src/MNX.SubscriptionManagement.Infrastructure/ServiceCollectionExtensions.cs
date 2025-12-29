using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MNX.Application.Data.EF.DI;
using MNX.SecurityManagement.Authentication.Clients;
using MNX.SubscriptionManagement.Application.Scheduler;
using MNX.SubscriptionManagement.Domain.Interfaces.Repositories;
using MNX.SubscriptionManagement.Infrastructure.DataBase;
using MNX.SubscriptionManagement.Infrastructure.DataBase.Repositories;
using MNX.SubscriptionManagement.Infrastructure.External;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.DebtRecording;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.LicenseExtension;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PostpaymentRenewal;
using MNX.SubscriptionManagement.Infrastructure.SagaStateMachine.PrepaymentRenewal;
using Quartz;

namespace MNX.SubscriptionManagement.Infrastructure;

/// <summary>
/// Расширения для <see cref="IServiceCollection"/> для регистрации модуля инфраструктуры.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавить инфраструктуру работу с данными.
    /// </summary>
    /// <param name="services"> Коллекция сервисов. </param>
    /// <param name="configuration"> Конфигурация. </param>
    /// <returns> Коллекция сервисов. </returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartzScheduler(configuration);
        ConfigureDI(services);

        var paymentUri = configuration["PaymentService"] ??
            throw new ArgumentNullException("Uri for payment service does not specified");

        services.AddHttpClient<IExternalPaymentRepository, ExternalPaymentRepository>(client =>
        {
            client.BaseAddress = new Uri(paymentUri);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddDataContext<Context>(configuration);

        services.AddMassTransit(configuration);

        return services;
    }

    private static IServiceCollection ConfigureDI(IServiceCollection services)
    {
        services.AddScoped<ISagaStatusesRepository, SagaStatusesRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ITariffPlanRepository, TariffPlanRepository>();
        services.AddScoped<IExternalPaymentRepository, ExternalPaymentRepository>();

        return services;
    }
    private static IServiceCollection AddQuartzScheduler(this IServiceCollection services, IConfiguration cfg)
    {
        ConfigureQuartz(services, cfg);

        // выполнение в бэкграунде
        services.AddQuartz(q =>
        {
            q.AddJob<SubscriptionExpiredJob>(SubscriptionExpiredJob.Key, j => j
                .StoreDurably()
                .WithDescription("Задача завершения подписки после окончания срока её действия")
            );
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    private static void ConfigureQuartz(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

        var jobStoreType = configuration["Quartz:quartz.jobStore.type"];

        // Конфигурируем только JobStoreTX (ADO.NET persistent storage)
        if (string.IsNullOrEmpty(jobStoreType) || jobStoreType != "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz") return;

        var quartzStoreOptions = GetStoreOptions(configuration);

        services.Configure<QuartzOptions>(options =>
        {
            foreach (var quartzOption in quartzStoreOptions)
                options.TryAdd(quartzOption.Key, quartzOption.Value);

            options.Scheduling.IgnoreDuplicates = true;
            options.Scheduling.OverWriteExistingData = true;
        });
    }

    private static Dictionary<string, string> GetStoreOptions(IConfiguration configuration)
    {
        const string defaultDataSource = "default";
        const string dataSourceProviderOption = $"quartz.dataSource.{defaultDataSource}.provider";
        const string driverDelegateTypeOption = "quartz.jobStore.driverDelegateType";
        const string connectionString = $"quartz.dataSource.{defaultDataSource}.connectionString";

        //Если конфигурация для Quartz задана в appsettings.json
        if (!string.IsNullOrEmpty(configuration["Quartz:quartz.jobStore.dataSource"]))
            return new Dictionary<string, string>();

        var provider = "Npgsql";
        var options = new Dictionary<string, string>
        {
            { "quartz.jobStore.dataSource", defaultDataSource },
            { $"quartz.dataSource.{defaultDataSource}.connectionStringName", provider.ToString() },

            { connectionString, configuration.GetConnectionString(provider)
                ?? throw new ArgumentNullException("Не задана строка подключения к СУБД.") },

            { dataSourceProviderOption, provider },
            { driverDelegateTypeOption, "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz" }
        };

        return options;
    }

    private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<Context>(x =>
            {
                x.UsePostgres();
                x.QueryDelay = TimeSpan.FromSeconds(1);
                x.UseBusOutbox();
            });

            x.AddSagaStateMachine<DebtRecordingSaga, DebtRecordingSagaState>()
                .EntityFrameworkRepository(x =>
                {
                    x.ExistingDbContext<Context>();
                    x.ConcurrencyMode = ConcurrencyMode.Optimistic;
                });

            x.AddSagaStateMachine<LicenseExtensionSaga, LicenseExtensionSagaState>()
                .EntityFrameworkRepository(x =>
                {
                    x.ExistingDbContext<Context>();
                    x.ConcurrencyMode = ConcurrencyMode.Optimistic;
                });

            x.AddSagaStateMachine<PostpaymentDebtRecordingSaga, PostpaymentDebtRecordingSagaState>()
                .EntityFrameworkRepository(x =>
                {
                    x.ExistingDbContext<Context>();
                    x.ConcurrencyMode = ConcurrencyMode.Optimistic;
                });

            x.AddSagaStateMachine<PrepaymentSubscriptionRenewalSaga, PrepaymentSubscriptionRenewalSagaState>()
                .EntityFrameworkRepository(x =>
                {
                    x.ExistingDbContext<Context>();
                    x.ConcurrencyMode = ConcurrencyMode.Optimistic;
                });

            x.AddConfigureEndpointsCallback((context, name, configs) =>
            {
                configs.UseEntityFrameworkOutbox<Context>(context);
            });

            x.UsingRabbitMq((context, configs) =>
            {
                configs.Host(configuration["RabbitMq:Host"], x =>
                {
                    x.Username(configuration["RabbitMQ:User"]!);
                    x.Password(configuration["RabbitMQ:Password"]!);
                });

                configs.UseMessageRetry(x =>
                {
                    x.Exponential(
                        retryLimit: 5,
                        minInterval: TimeSpan.FromSeconds(1),
                        maxInterval: TimeSpan.FromSeconds(30),
                        intervalDelta: TimeSpan.FromSeconds(5));
                });

                configs.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
