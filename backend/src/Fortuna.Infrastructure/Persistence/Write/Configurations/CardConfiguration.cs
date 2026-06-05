using Fortuna.Domain.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuna.Infrastructure.Persistence.Write.Configurations;

public sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.Property(x => x.CardType).HasColumnName("CardType").IsRequired();
        builder.Property(x => x.CreditLimit).HasColumnType("decimal(18,2)");
    }
}
