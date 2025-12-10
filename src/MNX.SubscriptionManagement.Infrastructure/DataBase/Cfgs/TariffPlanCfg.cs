using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase.Cfgs;

/// <summary>
/// Конфигурация сущности <see cref="TariffPlan"/> для базы данных.
/// </summary>
internal class TariffPlanCfg : IEntityTypeConfiguration<TariffPlan>
{
    public void Configure(EntityTypeBuilder<TariffPlan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                x => x.Value,
                value => new TariffId(value)
            );

        builder.Property(x => x.PaymentStrategy)
            .HasConversion<string>();
    }
}
