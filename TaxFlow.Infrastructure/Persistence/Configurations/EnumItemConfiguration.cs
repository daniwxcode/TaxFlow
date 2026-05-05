using Core.Domain.Contracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class EnumItemConfiguration : IEntityTypeConfiguration<EnumItem>
{
    public void Configure(EntityTypeBuilder<EnumItem> builder)
    {
        builder.ToTable("enum_items");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Label)
            .HasColumnName("label")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(e => e.Order)
            .HasColumnName("order");

        builder.Property(e => e.EnumDefinitionId)
            .HasColumnName("enum_definition_id")
            .IsRequired();

        builder.HasIndex(e => new { e.EnumDefinitionId, e.Code })
            .IsUnique()
            .HasDatabaseName("ux_enum_items_definition_code");

        builder.ConfigureAuditable();
    }
}
