using System;
using System.Collections.Generic;
using System.Linq;

using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class TaxObligationScheduleTests
{
    [Fact]
    public void Create_Returns_Empty_Schedule()
    {
        var schedule = TaxObligationSchedule.Create();

        Assert.False(schedule.HasDeclarationDeadline);
        Assert.False(schedule.HasPaymentDeadlines);
        Assert.Equal(0, schedule.InstallmentCount);
    }

    [Fact]
    public void WithDeclarationDeadline_Sets_Declaration()
    {
        var schedule = TaxObligationSchedule.Create();
        var deadline = DeclarationDeadline.Create("DECL", "Annual Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));

        schedule.WithDeclarationDeadline(deadline);

        Assert.True(schedule.HasDeclarationDeadline);
        Assert.Equal("DECL", schedule.DeclarationDeadline!.Key);
    }

    [Fact]
    public void AddPaymentDeadline_Adds_To_Collection()
    {
        var schedule = TaxObligationSchedule.Create();
        var payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        var payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.AddPaymentDeadline(payment1).AddPaymentDeadline(payment2);

        Assert.True(schedule.HasPaymentDeadlines);
        Assert.Equal(2, schedule.InstallmentCount);
    }

    [Fact]
    public void AddPaymentDeadline_Throws_On_Duplicate_Key()
    {
        var schedule = TaxObligationSchedule.Create();
        var payment1 = PaymentDeadline.Create("PAY", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero));
        var payment2 = PaymentDeadline.Create("PAY", "Duplicate Payment", new DateTimeOffset(2025, 5, 31, 0, 0, 0, TimeSpan.Zero));

        schedule.AddPaymentDeadline(payment1);

        Assert.Throws<InvalidOperationException>(() => schedule.AddPaymentDeadline(payment2));
    }

    [Fact]
    public void Validate_Detects_Fraction_Exceeds_100Percent()
    {
        var schedule = TaxObligationSchedule.Create();
        var payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.6m, 1);
        var payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.AddPaymentDeadline(payment1).AddPaymentDeadline(payment2);
        var result = schedule.Validate();

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Code == "INVALID_FRACTION_TOTAL");
    }

    [Fact]
    public void Validate_Detects_Declaration_After_FirstPayment()
    {
        var schedule = TaxObligationSchedule.Create();
        var declaration = DeclarationDeadline.Create("DECL", "Late Declaration", new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var payment = PaymentDeadline.Create("PAY", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero));

        schedule.WithDeclarationDeadline(declaration).AddPaymentDeadline(payment);
        var result = schedule.Validate();

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Code == "DECLARATION_AFTER_PAYMENT");
    }

    [Fact]
    public void Validate_Detects_Duplicate_Orders()
    {
        var schedule = TaxObligationSchedule.Create();
        var payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        var payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 1); // Same order

        schedule.AddPaymentDeadline(payment1).AddPaymentDeadline(payment2);
        var result = schedule.Validate();

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Code == "DUPLICATE_PAYMENT_ORDER");
    }

    [Fact]
    public void Validate_Succeeds_For_Valid_Schedule()
    {
        var schedule = TaxObligationSchedule.Create();
        var declaration = DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));
        var payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        var payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.WithDeclarationDeadline(declaration)
                .AddPaymentDeadline(payment1)
                .AddPaymentDeadline(payment2);

        var result = schedule.Validate();

        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetOverdueDeadlines_Returns_Only_Overdue()
    {
        var schedule = TaxObligationSchedule.Create();
        var declaration = DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));
        var payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        var payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.WithDeclarationDeadline(declaration)
                .AddPaymentDeadline(payment1)
                .AddPaymentDeadline(payment2);

        var asOf = new DateTimeOffset(2025, 5, 15, 0, 0, 0, TimeSpan.Zero);
        var overdue = schedule.GetOverdueDeadlines(asOf);

        Assert.Equal(2, overdue.Count); // Declaration and first payment are overdue
        Assert.Contains(overdue, d => d.Key == "DECL");
        Assert.Contains(overdue, d => d.Key == "PAY1");
        Assert.DoesNotContain(overdue, d => d.Key == "PAY2");
    }

    [Fact]
    public void PaymentDeadline_GetAmountDue_Calculates_Fraction()
    {
        var payment = PaymentDeadline.Create("PAY", "Payment", DateTimeOffset.Now, 0.25m, 1);

        var amountDue = payment.GetAmountDue(100000m);

        Assert.Equal(25000m, amountDue);
    }

    [Fact]
    public void DeclarationDeadline_WithPenalty_Attaches_Definition()
    {
        var deadline = DeclarationDeadline.Create("DECL", "Declaration", DateTimeOffset.Now)
            .WithPenalty(new PenaltyDefinition
            {
                Type = PenaltyType.Assiette,
                FixedAmount = 100m,
                AnnualRate = 0.12m
            });

        Assert.NotNull(deadline.PenaltyDefinition);
        Assert.Equal(100m, deadline.PenaltyDefinition.FixedAmount);
    }

    [Fact]
    public void PaymentDeadline_WithPenalty_Attaches_Definition()
    {
        var deadline = PaymentDeadline.Create("PAY", "Payment", DateTimeOffset.Now)
            .WithPenalty(new PenaltyDefinition
            {
                Type = PenaltyType.Recouvrement,
                AnnualRate = 0.10m
            });

        Assert.NotNull(deadline.PenaltyDefinition);
        Assert.Equal(0.10m, deadline.PenaltyDefinition.AnnualRate);
    }
}
