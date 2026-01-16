using System;
using System.Collections.Generic;
using System.Linq;

using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class ObligationPenaltyCalculatorTests
{
    [Fact]
    public void Calculate_Returns_Empty_When_No_Schedule()
    {
        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var calculator = ObligationPenaltyCalculator.Default;

        var result = calculator.Calculate(rule, 100000m, DateTimeOffset.Now);

        Assert.Empty(result.AllPenalties);
        Assert.Equal(0m, result.TotalAmount);
    }

    [Fact]
    public void Calculate_Returns_Empty_When_No_Overdue_Deadlines()
    {
        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var schedule = TaxObligationSchedule.Create()
            .WithDeclarationDeadline(
                DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero))
                    .WithPenalty(new PenaltyDefinition { Type = PenaltyType.Assiette, FixedAmount = 100m }));

        rule.ConfigureObligationSchedule(schedule);

        var asOf = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var result = ObligationPenaltyCalculator.Default.Calculate(rule, 100000m, asOf);

        Assert.Empty(result.AllPenalties);
    }

    [Fact]
    public void Calculate_Declaration_FixedPenalty()
    {
        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var schedule = TaxObligationSchedule.Create()
            .WithDeclarationDeadline(
                DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero))
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Assiette,
                        FixedAmount = 500m
                    }));

        rule.ConfigureObligationSchedule(schedule);

        var asOf = new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero);
        var result = ObligationPenaltyCalculator.Default.Calculate(rule, 100000m, asOf);

        Assert.Single(result.DeclarationPenalties);
        Assert.Equal(500m, result.TotalDeclarationPenalty);
        Assert.Equal(PenaltyLineType.AssietteFixed, result.DeclarationPenalties[0].LineType);
    }

    [Fact]
    public void Calculate_Declaration_PeriodicPenalty()
    {
        var policy = new PenaltyPolicy { DaysInYear = 360 };
        var calculator = new ObligationPenaltyCalculator(policy);

        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var schedule = TaxObligationSchedule.Create()
            .WithDeclarationDeadline(
                DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Assiette,
                        AnnualRate = 0.12m,
                        PeriodDays = 30
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // 60 days late = 2 periods
        var asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        var result = calculator.Calculate(rule, 1000m, asOf);

        Assert.Equal(2, result.DeclarationPenalties.Count);
        // Each period: 1000 * 0.12 * 30 / 360 = 10
        Assert.All(result.DeclarationPenalties, p => Assert.Equal(PenaltyLineType.AssietteRate, p.LineType));
    }

    [Fact]
    public void Calculate_Payment_PeriodicPenalty_On_Outstanding()
    {
        var policy = new PenaltyPolicy { DaysInYear = 360 };
        var calculator = new ObligationPenaltyCalculator(policy);

        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 1)
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Recouvrement,
                        AnnualRate = 0.12m,
                        PeriodDays = 30
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // 50% of 10000 = 5000 due, 30 days late = 1 period
        var asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        var result = calculator.Calculate(rule, 10000m, asOf);

        Assert.True(result.PaymentPenalties.ContainsKey("PAY1"));
        Assert.NotEmpty(result.PaymentPenalties["PAY1"]);
        Assert.All(result.PaymentPenalties["PAY1"], p => Assert.Equal(PenaltyLineType.RecouvrementRate, p.LineType));
    }

    [Fact]
    public void Calculate_Payment_Reduces_Penalty_When_PartiallyPaid()
    {
        var policy = new PenaltyPolicy { DaysInYear = 360 };
        var calculator = new ObligationPenaltyCalculator(policy);

        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 1.0m, 1)
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Recouvrement,
                        PeriodRate = 0.10m,
                        PeriodDays = 30
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // 1000 due, 500 paid, 500 outstanding
        var payments = new Dictionary<string, decimal> { { "PAY1", 500m } };
        var asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        var result = calculator.Calculate(rule, 1000m, asOf, payments);

        Assert.True(result.PaymentPenalties.ContainsKey("PAY1"));
        // Penalty should be 10% of 500 = 50 per period
        var firstPenalty = result.PaymentPenalties["PAY1"].First();
        Assert.Equal(50m, firstPenalty.Amount);
    }

    [Fact]
    public void Calculate_No_Penalty_When_FullyPaid()
    {
        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 1.0m, 1)
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Recouvrement,
                        PeriodRate = 0.10m
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // Fully paid
        var payments = new Dictionary<string, decimal> { { "PAY1", 1000m } };
        var asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        var result = ObligationPenaltyCalculator.Default.Calculate(rule, 1000m, asOf, payments);

        Assert.Empty(result.PaymentPenalties);
        Assert.Equal(0m, result.TotalPaymentPenalty);
    }

    [Fact]
    public void Calculate_Multiple_Payment_Deadlines()
    {
        var rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        var schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 1)
                    .WithPenalty(new PenaltyDefinition { Type = PenaltyType.Recouvrement, PeriodRate = 0.05m, PeriodDays = 30 }))
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 2)
                    .WithPenalty(new PenaltyDefinition { Type = PenaltyType.Recouvrement, PeriodRate = 0.10m, PeriodDays = 30 }));

        rule.ConfigureObligationSchedule(schedule);

        // Both deadlines overdue
        var asOf = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var result = ObligationPenaltyCalculator.Default.Calculate(rule, 10000m, asOf);

        Assert.True(result.PaymentPenalties.ContainsKey("PAY1"));
        Assert.True(result.PaymentPenalties.ContainsKey("PAY2"));
    }
}
