using Fortuna.ReadModel.Dashboard.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.ReadModel.Persistence.Configurations;

public sealed class TimelineEventReadModelConfiguration : IEntityTypeConfiguration<TimelineEventReadModel>
{
    public void Configure(EntityTypeBuilder<TimelineEventReadModel> builder)
    {
        builder.ToTable("TimelineEvent");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.ProductId);
        builder.Property(x => x.EventDateUtc).IsRequired();
        builder.Property(x => x.EventType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.IsPositive).IsRequired();

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.EventDateUtc);
    }
}
