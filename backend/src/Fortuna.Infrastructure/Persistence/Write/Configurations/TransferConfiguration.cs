using Fortuna.Domain.Accounts;
using Fortuna.Domain.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.Infrastructure.Persistence.Write.Configurations;

public sealed class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable("Transfers", "dbo");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, value => new TransferId(value));

        builder.Property(x => x.TransferType).IsRequired();
        builder.OwnsOne(x => x.Amount, owned =>
        {
            owned.Property(p => p.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
            owned.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });

        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.ExternalTargetAccountNumber).HasMaxLength(34);
        builder.Property(x => x.ExternalRecipientName).HasMaxLength(200);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CompletedAtUtc);
    }
}
