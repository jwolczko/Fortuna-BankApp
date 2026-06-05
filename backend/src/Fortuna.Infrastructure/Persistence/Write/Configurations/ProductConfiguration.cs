using Fortuna.Domain.Accounts;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Loans;
using Fortuna.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.Infrastructure.Persistence.Write.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "dbo");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CustomerId)
            .HasConversion(x => x.Value, value => new CustomerId(value));

        builder.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ProductNumber).HasMaxLength(64).IsRequired();
        builder.Property(x => x.NumberSequence).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Category).HasColumnName("ProductCategory").IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.OwnsOne(x => x.Balance, owned =>
        {
            owned.Property(p => p.Amount).HasColumnName("Balance").HasColumnType("decimal(18,2)").IsRequired();
            owned.Property(p => p.Currency).HasColumnName("BalanceCurrency").HasMaxLength(3).IsRequired();
        });

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<BankAccount>("BankAccount")
            .HasValue<Card>("Card")
            .HasValue<Loan>("Loan");
        builder.HasOne<Customer>()
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CustomerId);

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.ProductNumber).IsUnique();
        builder.HasIndex(x => x.NumberSequence).IsUnique();
    }
}
