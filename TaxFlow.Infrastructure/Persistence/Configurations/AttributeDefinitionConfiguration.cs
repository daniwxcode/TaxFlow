using Core.Domain.Contracts;
using Core.Domain.Enums;

using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class AttributeDefinitionConfiguration : IEntityTypeConfiguration<AttributeDefinition>
{
    public void Configure(EntityTypeBuilder<AttributeDefinition> builder)
    {
        builder.ToTable("attribute_definitions");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id");

        builder.Property(a => a.Key)
            .HasColumnName("key")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(a => a.Label)
            .HasColumnName("label")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(a => a.DataType)
            .HasColumnName("data_type")
            .HasConversion(
                value => value.Value,
                value => AttributeDataType.FromValue(value))
            .IsRequired();

        builder.Property(a => a.IsRequired)
            .HasColumnName("is_required")
            .IsRequired();

        builder.Property(a => a.RegexPattern)
            .HasColumnName("regex_pattern")
            .HasMaxLength(2000);

        builder.Property(a => a.EnumDefinitionId)
            .HasColumnName("enum_definition_id");

        builder.Property<Guid>("asset_type_id")
            .HasColumnName("asset_type_id")
            .IsRequired();

        builder.HasOne(a => a.EnumDefinition)
            .WithMany()
            .HasForeignKey(a => a.EnumDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex("asset_type_id", nameof(AttributeDefinition.Key))
            .IsUnique()
            .HasDatabaseName("ux_attribute_definitions_asset_key");

        builder.HasIndex(a => a.EnumDefinitionId)
            .HasDatabaseName("ix_attribute_definitions_enum_definition_id");

        builder.ConfigureAuditable();
    }
}
