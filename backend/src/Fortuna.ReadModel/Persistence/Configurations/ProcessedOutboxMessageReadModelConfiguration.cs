using Fortuna.ReadModel.Dashboard.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.ReadModel.Persistence.Configurations;

public sealed class ProcessedOutboxMessageReadModelConfiguration : IEntityTypeConfiguration<ProcessedOutboxMessageReadModel>
{
    public void Configure(EntityTypeBuilder<ProcessedOutboxMessageReadModel> builder)
    {
        builder.ToTable("ProcessedOutboxMessage");
        builder.HasKey(x => x.OutboxMessageId);

        builder.Property(x => x.OutboxMessageId).ValueGeneratedNever();
        builder.Property(x => x.ProcessedAtUtc).IsRequired();
    }
}
