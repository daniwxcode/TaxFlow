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

        builder.HasMany(s => s.DeclarationDeadlines)
            .WithOne()
            .HasForeignKey("schedule_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.PaymentDeadlines)
            .WithOne()
            .HasForeignKey("schedule_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.DeclarationDeadlines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(s => s.PaymentDeadlines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.LegalReferences)
            .WithMany()
            .UsingEntity(j => j.ToTable("tax_obligation_schedule_legal_references"));

        builder.Navigation(s => s.LegalReferences)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
