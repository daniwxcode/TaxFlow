using Core.Domain.Tax.Assets;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class TaxableAssetConfiguration : IEntityTypeConfiguration<TaxableAsset>
{
    public void Configure(EntityTypeBuilder<TaxableAsset> builder)
    {
        builder.ToTable("taxable_assets");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id");

        builder.Property(a => a.AssetTypeId)
            .HasColumnName("asset_type_id")
            .IsRequired();

        builder.Property(a => a.ExternalId)
            .HasColumnName("external_id")
            .HasMaxLength(200);

        builder.Property(a => a.ValidFrom)
            .HasColumnName("valid_from")
            .IsRequired();

        builder.Property(a => a.ValidTo)
            .HasColumnName("valid_to");

        builder.HasOne(a => a.AssetType)
            .WithMany()
            .HasForeignKey(a => a.AssetTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Attributes)
            .WithOne()
            .HasForeignKey("asset_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(a => a.AssetTypeId)
            .HasDatabaseName("ix_taxable_assets_asset_type_id");

        builder.HasIndex(a => a.ExternalId)
            .HasDatabaseName("ix_taxable_assets_external_id");

        builder.ConfigureAuditable();
        builder.ConfigureSoftDelete();
    }
}
