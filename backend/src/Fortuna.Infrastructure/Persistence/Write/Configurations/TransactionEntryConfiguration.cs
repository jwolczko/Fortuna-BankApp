using Fortuna.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.Infrastructure.Persistence.Write.Configurations;

public sealed class TransactionEntryConfiguration : IEntityTypeConfiguration<TransactionEntry>
{
    public void Configure(EntityTypeBuilder<TransactionEntry> builder)
    {
        builder.ToTable("Transactions", "dbo");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, value => new TransactionId(value));

        builder.OwnsOne(x => x.Amount, owned =>
        {
            owned.Property(p => p.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
            owned.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });

        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.BookedAtUtc).IsRequired();
    }
}
