using Core.Domain.Contracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class ExtendedAttributeConfiguration : IEntityTypeConfiguration<ExtendedAttribute>
{
    public void Configure(EntityTypeBuilder<ExtendedAttribute> builder)
    {
        builder.ToTable("extended_attributes");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id");

        builder.Property(a => a.Key)
            .HasColumnName("key")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(a => a.Value)
            .HasColumnName("value")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(a => a.DataTypeValue)
            .HasColumnName("data_type")
            .IsRequired();

        builder.Property(a => a.IsRequired)
            .HasColumnName("is_required")
            .IsRequired();

        builder.Property(a => a.ValidFrom)
            .HasColumnName("valid_from")
            .IsRequired();

        builder.Property(a => a.ValidTo)
            .HasColumnName("valid_to");

        builder.Property<Guid>("asset_id")
            .HasColumnName("asset_id")
            .IsRequired();

        builder.HasIndex("asset_id")
            .HasDatabaseName("ix_extended_attributes_asset_id");

        builder.HasIndex(a => a.Key)
            .HasDatabaseName("ix_extended_attributes_key");

        builder.ConfigureAuditable();
        builder.ConfigureSoftDelete();
    }
}
