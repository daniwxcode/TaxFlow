using Core.Domain.Tax.Calculation;

using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class TaxRuleConfiguration : IEntityTypeConfiguration<TaxRule>
{
    public void Configure(EntityTypeBuilder<TaxRule> builder)
    {
        builder.ToTable("tax_rules");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.Key)
            .HasColumnName("key")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(r => r.Label)
            .HasColumnName("label")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(r => r.Expression)
            .HasColumnName("expression")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(r => r.Enabled)
            .HasColumnName("enabled")
            .IsRequired();

        builder.Property<Guid>("asset_type_id")
            .HasColumnName("asset_type_id")
            .IsRequired();

        builder.ConfigureAuditable();
        builder.ConfigureTemporal();

        builder.HasIndex("asset_type_id", nameof(TaxRule.Key))
            .IsUnique()
            .HasDatabaseName("ux_tax_rules_asset_key");

        builder.HasOne(r => r.ObligationSchedule)
            .WithOne()
            .HasForeignKey<Core.Domain.Tax.Obligations.TaxObligationSchedule>("tax_rule_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
