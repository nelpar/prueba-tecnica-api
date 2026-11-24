using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaTecnica.Domain.Entities;
using PruebaTecnica.Infrastructure.Persistence.Seed;

namespace PruebaTecnica.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(3)
            .UseCollation("NOCASE");
        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.RateToBase).IsRequired();

        builder.HasData(CurrencySeed.Data);
    }
}
