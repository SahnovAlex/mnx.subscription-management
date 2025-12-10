using Microsoft.EntityFrameworkCore;
using MNX.SubscriptionManagement.Domain.Core;
using System.Reflection;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase;

/// <summary>
/// Контекст данных.
/// </summary>
public class Context : DbContext
{
    internal DbSet<Subscription> Subscriptions { get; set; }

    internal DbSet<TariffPlan> TariffPlans { get; set; }

    internal DbSet<SagaOperation> SagaOperationStatuses { get; set; }

    public Context(DbContextOptions<Context> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
