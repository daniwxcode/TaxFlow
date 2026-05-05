using Core.Domain.Tax.Obligations;

using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class TaxObligationScheduleConfiguration : IEntityTypeConfiguration<TaxObligationSchedule>
{
    public void Configure(EntityTypeBuilder<TaxObligationSchedule> builder)
    {
        builder.ToTable("tax_obligation_schedules");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id");

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(250);

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(s => s.FiscalYear)
            .HasColumnName("fiscal_year");

        builder.Property<Guid>("tax_rule_id")
            .HasColumnName("tax_rule_id")
            .IsRequired();

        builder.ConfigureAuditable();

        builder.HasMany<DeclarationDeadline>("_declarationDeadlines")
            .WithOne()
            .HasForeignKey("schedule_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<PaymentDeadline>("_paymentDeadlines")
            .WithOne()
            .HasForeignKey("schedule_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation("_declarationDeadlines")
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation("_paymentDeadlines")
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<LegalReference>("_legalReferences")
            .WithMany()
            .UsingEntity(j => j.ToTable("tax_obligation_schedule_legal_references"));

        builder.Metadata.FindNavigation("_legalReferences")
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
