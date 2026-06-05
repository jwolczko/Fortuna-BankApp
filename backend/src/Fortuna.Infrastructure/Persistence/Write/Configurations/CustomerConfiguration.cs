using Fortuna.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.Infrastructure.Persistence.Write.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "dbo");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, value => new CustomerId(value));

        builder.OwnsOne(x => x.FullName, owned =>
        {
            owned.Property(p => p.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            owned.Property(p => p.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
        });

        builder.OwnsOne(x => x.Email, owned =>
        {
            owned.Property(p => p.Value).HasColumnName("Email").HasMaxLength(200).IsRequired();
            owned.HasIndex(p => p.Value).IsUnique();
        });

        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Type).HasColumnName("CustomerType").IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.Metadata.FindNavigation(nameof(Customer.Products))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
