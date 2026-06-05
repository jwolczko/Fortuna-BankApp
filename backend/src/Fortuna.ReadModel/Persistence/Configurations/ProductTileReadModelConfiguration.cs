using Fortuna.ReadModel.Dashboard.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.ReadModel.Persistence.Configurations;

public sealed class ProductTileReadModelConfiguration : IEntityTypeConfiguration<ProductTileReadModel>
{
    public void Configure(EntityTypeBuilder<ProductTileReadModel> builder)
    {
        builder.ToTable("ProductTile");
        builder.HasKey(x => x.ProductId);

        builder.Property(x => x.ProductId).ValueGeneratedNever();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.ProductCategory).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ProductType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ProductNumber).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Balance).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();

        builder.HasIndex(x => x.CustomerId);
    }
}
