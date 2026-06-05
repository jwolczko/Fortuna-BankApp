using Fortuna.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.Infrastructure.Persistence.Write.Configurations;

public sealed class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.Property(x => x.AccountType).HasColumnName("BankAccountType").IsRequired();

        builder.OwnsOne(x => x.AccountNumber, owned =>
        {
            owned.Property(p => p.Value).HasColumnName("AccountNumber").HasMaxLength(34).IsRequired();
        });

        builder.HasMany(x => x.Transactions)
            .WithOne()
            .HasForeignKey(x => x.BankAccountId);

        builder.Metadata.FindNavigation(nameof(BankAccount.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
