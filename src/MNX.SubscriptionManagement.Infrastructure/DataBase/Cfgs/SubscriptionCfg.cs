using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase.Cfgs;

/// <summary>
/// Конфигурация сущности <see cref="Subscription"/> для базы данных.
/// </summary>
internal class SubscriptionCfg : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                x => x.Value,
                value => new SubscriptionId(value)
            );

        builder.Property(x => x.UserId)
            .HasConversion(
                x => x.Value,
                value => new UserId(value)
            );

        builder.Property(x => x.TariffPlanId)
            .HasConversion(
                x => x.Value,
                value => new TariffId(value)
            );

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.StartDateTime)
            .HasConversion(
                x => x.UtcDateTime,
                x => new DateTimeOffset(x, TimeSpan.Zero)
            );

        builder.Property(x => x.EndDateTime)
            .HasConversion(
                x => x.UtcDateTime,
                x => new DateTimeOffset(x, TimeSpan.Zero)
            );
    }
}
