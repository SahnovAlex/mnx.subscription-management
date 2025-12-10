using MNX.Application.Consul;
using MNX.Application.CustomMiddlewares;
using MNX.Application.OpenTelemetry;
using MNX.Application.OpenTelemetry.Metrics;
using MNX.Application.OpenTelemetry.Tracing;
using MNX.SecurityManagement.Authentication.Integration;
using MNX.SecurityManagement.Authorization.Integration;
using MNX.SubscriptionManagement.Application;
using MNX.SubscriptionManagement.Infrastructure;
using NLog;
using NLog.Web;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace MNX.SubscriptionManagement.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var logger = LogManager.Setup()
            .LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        try
        {
            logger.Debug("init main");
            var builder = ConfigureApp(args);
            await RunApp(builder);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while starting the host");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    private static WebApplicationBuilder ConfigureApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Encoder =
                        JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
                });

        services.AddEndpointsApiExplorer();

        services.AddConsulIntegration(configuration);

        services.ConfigureOpenTelemetry(configuration)
                .ConfigureTracing(configuration)
                .ConfigureMetrics(configuration);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddJwtBearerAuthentication(configuration["SecretKey"]!);

        services.AddServiceUserAuth(configuration);
        services.AddWhiteListBasedAuthorization(configuration);

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        services.AddSwagger(xmlFilePath);

        ConfigureDI(services, configuration);

        return builder;
    }

    private static void ConfigureDI(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddApplication();
    }

    private static Task RunApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();
        var appName = builder.Configuration["ServiceName"]
            ?? throw new ArgumentNullException(null, "Service name does not specified");

        //if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandlingMiddleware();

        app.MapControllers();
        app.MapHealthChecks("/health").AllowAnonymous();
        app.MapGet(string.Empty, async ctx => await ctx.Response.WriteAsync(appName)).AllowAnonymous();

        return app.RunAsync();
    }
}