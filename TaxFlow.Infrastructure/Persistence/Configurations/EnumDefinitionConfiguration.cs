using Core.Domain.Contracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class EnumDefinitionConfiguration : IEntityTypeConfiguration<EnumDefinition>
{
    public void Configure(EntityTypeBuilder<EnumDefinition> builder)
    {
        builder.ToTable("enum_definitions");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Key)
            .HasColumnName("key")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.Label)
            .HasColumnName("label")
            .HasMaxLength(250)
            .IsRequired();

        builder.HasIndex(e => e.Key)
            .IsUnique()
            .HasDatabaseName("ux_enum_definitions_key");

        builder.ConfigureAuditable();

        builder.HasMany(e => e.Items)
            .WithOne(i => i.EnumDefinition)
            .HasForeignKey(i => i.EnumDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
