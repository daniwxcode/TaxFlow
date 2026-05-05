using Core.Domain.Tax.Obligations;

using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class PaymentDeadlineConfiguration : IEntityTypeConfiguration<PaymentDeadline>
{
    public void Configure(EntityTypeBuilder<PaymentDeadline> builder)
    {
        builder.ToTable("payment_deadlines");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id");

        builder.Property(d => d.Key)
            .HasColumnName("key")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(d => d.Label)
            .HasColumnName("label")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property<Guid>("schedule_id")
            .HasColumnName("schedule_id")
            .IsRequired();

        builder.Property(d => d.DueDate)
            .HasColumnName("due_date")
            .IsRequired();

        builder.Property(d => d.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(d => d.Enabled)
            .HasColumnName("enabled")
            .IsRequired();

        builder.Property(d => d.Periodicity)
            .HasColumnName("periodicity")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.Regime)
            .HasColumnName("regime")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.Order)
            .HasColumnName("order")
            .IsRequired();

        builder.Property(d => d.ConditionExpression)
            .HasColumnName("condition_expression")
            .HasMaxLength(2000);

        builder.Property(d => d.FiscalYear)
            .HasColumnName("fiscal_year");

        builder.Property(d => d.Period)
            .HasColumnName("period");

        builder.Property(d => d.Fraction)
            .HasColumnName("fraction")
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(d => d.PaymentType)
            .HasColumnName("payment_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.LinkedDeclarationKey)
            .HasColumnName("linked_declaration_key")
            .HasMaxLength(150);

        builder.Property(d => d.AllowsPartialPayment)
            .HasColumnName("allows_partial_payment")
            .IsRequired();

        builder.Property(d => d.MinimumPayment)
            .HasColumnName("minimum_payment")
            .HasPrecision(18, 2);

        builder.Property(d => d.FixedAmount)
            .HasColumnName("fixed_amount")
            .HasPrecision(18, 2);

        builder.Property(d => d.LocalizedLabel)
            .HasColumnName("localized_label")
            .HasColumnType("jsonb")
            .HasConversion(LocalizedStringConversion.JsonConverter)
            .Metadata.SetValueComparer(LocalizedStringConversion.JsonComparer);

        builder.Property(d => d.LocalizedDescription)
            .HasColumnName("localized_description")
            .HasColumnType("jsonb")
            .HasConversion(LocalizedStringConversion.JsonConverter)
            .Metadata.SetValueComparer(LocalizedStringConversion.JsonComparer);

        builder.Property(d => d.GracePeriod)
            .HasColumnName("grace_period")
            .HasColumnType("jsonb")
            .HasConversion(DurationConversion.JsonConverter)
            .Metadata.SetValueComparer(DurationConversion.JsonComparer);

        builder.OwnsOne(d => d.PenaltyDefinition, pd =>
        {
            pd.Property(p => p.Type)
                .HasColumnName("penalty_type")
                .HasConversion<int>();
            pd.Property(p => p.TriggerEvent)
                .HasColumnName("penalty_trigger_event")
                .HasConversion<int>();
            pd.Property(p => p.FixedAmount)
                .HasColumnName("penalty_fixed_amount");
            pd.Property(p => p.AnnualRate)
                .HasColumnName("penalty_annual_rate");
            pd.Property(p => p.PeriodRate)
                .HasColumnName("penalty_period_rate");
            pd.Property(p => p.PeriodRateIncrement)
                .HasColumnName("penalty_period_rate_increment");
            pd.Property(p => p.Cap)
                .HasColumnName("penalty_cap");
            pd.Property(p => p.Minimum)
                .HasColumnName("penalty_minimum");
            pd.Property(p => p.Capitalize)
                .HasColumnName("penalty_capitalize");

            pd.Property(p => p.GracePeriod)
                .HasColumnName("penalty_grace_period")
                .HasColumnType("jsonb")
                .HasConversion(DurationConversion.JsonConverter)
                .Metadata.SetValueComparer(DurationConversion.JsonComparer);

            pd.Property(p => p.Period)
                .HasColumnName("penalty_period")
                .HasColumnType("jsonb")
                .HasConversion(DurationConversion.JsonConverter)
                .Metadata.SetValueComparer(DurationConversion.JsonComparer);
        });

        builder.Navigation(d => d.PenaltyDefinition)
            .IsRequired(false);

        builder.ConfigureAuditable();

        builder.HasMany(d => d.LegalReferences)
            .WithMany()
            .UsingEntity(j => j.ToTable("payment_deadline_legal_references"));

        builder.Navigation(d => d.LegalReferences)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
