using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class AssetTypeConfiguration : IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> builder)
    {
        builder.ToTable("asset_types");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id");

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(a => a.LiquidationMode)
            .HasColumnName("liquidation_mode")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(a => a.Name)
            .HasDatabaseName("ix_asset_types_name");

        builder.ConfigureAuditable();
        builder.ConfigureSoftDelete();

        builder.HasMany<AttributeDefinition>("_expectedAttributes")
            .WithOne()
            .HasForeignKey("asset_type_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<TaxRule>("_taxRules")
            .WithOne()
            .HasForeignKey("asset_type_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation("_expectedAttributes")
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation("_taxRules")
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
