using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MNX.SubscriptionManagement.Domain.Core;
using MNX.SubscriptionManagement.Domain.Core.ValueObjects;

namespace MNX.SubscriptionManagement.Infrastructure.DataBase.Cfgs;

/// <summary>
/// Конфигурация сущности <see cref="SagaOperation"/> для базы данных.
/// </summary>
internal class SagaOperationStatusCfg : IEntityTypeConfiguration<SagaOperation>
{
    public void Configure(EntityTypeBuilder<SagaOperation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new OperationId(value)
            );

        builder.Property(x => x.Status)
            .HasConversion<string>();
    }
}
