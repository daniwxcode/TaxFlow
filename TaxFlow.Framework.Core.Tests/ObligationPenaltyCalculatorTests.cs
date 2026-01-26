using System;
using System.Collections.Generic;
using System.Linq;

using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;
/// <summary>
/// Contains unit tests for the ObligationPenaltyCalculator, verifying penalty calculations for tax obligations and
/// payments under various scenarios.
/// </summary>
/// <remarks>These tests cover cases such as missing schedules, overdue deadlines, fixed and periodic penalties,
/// partial and full payments, and multiple payment deadlines. The tests ensure that the calculator correctly computes
/// penalties based on the provided tax rules, schedules, payment information, and dates.</remarks>
public class ObligationPenaltyCalculatorTests
{
    /// <summary>
    /// Ensures that when no schedule is configured the calculator returns an empty result set.
    /// </summary>
    /// <remarks>Validates the default behavior when a tax rule lacks declaration or payment deadlines.</remarks>
    [Fact]
    public void Calculate_Returns_Empty_When_No_Schedule()
    {
        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        ObligationPenaltyCalculator calculator = ObligationPenaltyCalculator.Default;

        ObligationPenaltyResult result = calculator.Calculate(rule, 100000m, DateTimeOffset.Now);

        Assert.Empty(result.AllPenalties);
        Assert.Equal(0m, result.TotalAmount);
    }
    /// <summary>
    /// Verifies that the penalty calculator returns an empty result when no declaration deadlines are overdue as of the
    /// specified date.
    /// </summary>
    /// <remarks>This test ensures that penalties are not applied when all declaration deadlines are in the
    /// future relative to the calculation date. It helps confirm correct behavior for non-overdue scenarios.</remarks>
    [Fact]
    public void Calculate_Returns_Empty_When_No_Overdue_Deadlines()
    {
        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        TaxObligationSchedule schedule = TaxObligationSchedule.Create()
            .WithDeclarationDeadline(
                DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero))
                    .WithPenalty(new PenaltyDefinition { Type = PenaltyType.Assiette, FixedAmount = 100m }));

        rule.ConfigureObligationSchedule(schedule);

        DateTimeOffset asOf = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero);
        ObligationPenaltyResult result = ObligationPenaltyCalculator.Default.Calculate(rule, 100000m, asOf);

        Assert.Empty(result.AllPenalties);
    }
    /// <summary>
    /// Verifies that a fixed penalty tied to a declaration deadline is returned when overdue.
    /// </summary>
    /// <remarks>Uses a single declaration deadline with a fixed penalty to assert the computed amount.</remarks>
    [Fact]
    public void Calculate_Declaration_FixedPenalty()
    {
        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        TaxObligationSchedule schedule = TaxObligationSchedule.Create()
            .WithDeclarationDeadline(
                DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero))
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Assiette,
                        FixedAmount = 500m
                    }));

        rule.ConfigureObligationSchedule(schedule);

        DateTimeOffset asOf = new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero);
        ObligationPenaltyResult result = ObligationPenaltyCalculator.Default.Calculate(rule, 100000m, asOf);

        Assert.Single(result.DeclarationPenalties);
        Assert.Equal(500m, result.TotalDeclarationPenalty);
        Assert.Equal(PenaltyLineType.AssietteFixed, result.DeclarationPenalties[0].LineType);
    }

    /// <summary>
    /// Confirms that periodic declaration penalties create one line per elapsed period.
    /// </summary>
    /// <remarks>Leverages a policy with custom days in year to check proportional penalty accrual.</remarks>
    [Fact]
    public void Calculate_Declaration_PeriodicPenalty()
    {
        PenaltyPolicy policy = new PenaltyPolicy { DaysInYear = 360 };
        ObligationPenaltyCalculator calculator = new ObligationPenaltyCalculator(policy);

        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        TaxObligationSchedule schedule = TaxObligationSchedule.Create()
            .WithDeclarationDeadline(
                DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Assiette,
                        AnnualRate = 0.12m,
                        Period = Duration.Days(30)
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // 60 days late = 2 periods
        DateTimeOffset asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        ObligationPenaltyResult result = calculator.Calculate(rule, 1000m, asOf);

        Assert.Equal(2, result.DeclarationPenalties.Count);
        // Each period: 1000 * 0.12 * 30 / 360 = 10
        Assert.All(result.DeclarationPenalties, p => Assert.Equal(PenaltyLineType.AssietteRate, p.LineType));
    }

    /// <summary>
    /// Ensures payment penalties accrue on outstanding balances for overdue payment deadlines.
    /// </summary>
    /// <remarks>Focuses on recouvrement penalties produced for a single overdue payment deadline.</remarks>
    [Fact]
    public void Calculate_Payment_PeriodicPenalty_On_Outstanding()
    {
        PenaltyPolicy policy = new PenaltyPolicy { DaysInYear = 360 };
        ObligationPenaltyCalculator calculator = new ObligationPenaltyCalculator(policy);

        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        TaxObligationSchedule schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 1)
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Recouvrement,
                        AnnualRate = 0.12m,
                        Period = Duration.Days(30)
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // 50% of 10000 = 5000 due, 30 days late = 1 period
        DateTimeOffset asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        ObligationPenaltyResult result = calculator.Calculate(rule, 10000m, asOf);

        Assert.True(result.PaymentPenalties.ContainsKey("PAY1"));
        Assert.NotEmpty(result.PaymentPenalties["PAY1"]);
        Assert.All(result.PaymentPenalties["PAY1"], p => Assert.Equal(PenaltyLineType.RecouvrementRate, p.LineType));
    }

    /// <summary>
    /// Verifies that recorded payments reduce subsequent penalty calculations proportionally.
    /// </summary>
    /// <remarks>Simulates a partial payment to ensure penalties apply only to the outstanding fraction.</remarks>
    [Fact]
    public void Calculate_Payment_Reduces_Penalty_When_PartiallyPaid()
    {
        PenaltyPolicy policy = new PenaltyPolicy { DaysInYear = 360 };
        ObligationPenaltyCalculator calculator = new ObligationPenaltyCalculator(policy);

        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        TaxObligationSchedule schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 1.0m, 1)
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Recouvrement,
                        PeriodRate = 0.10m,
                        Period = Duration.Days(30)
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // 1000 due, 500 paid, 500 outstanding
        Dictionary<string, decimal> payments = new Dictionary<string, decimal> { { "PAY1", 500m } };
        DateTimeOffset asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        ObligationPenaltyResult result = calculator.Calculate(rule, 1000m, asOf, payments);

        Assert.True(result.PaymentPenalties.ContainsKey("PAY1"));
        // Penalty should be 10% of 500 = 50 per period
        PenaltyAccrual firstPenalty = result.PaymentPenalties["PAY1"][0];
        Assert.Equal(50m, firstPenalty.Amount);
    }

    /// <summary>
    /// Checks that no payment penalties are returned when the obligation is fully settled.
    /// </summary>
    /// <remarks>Asserts that an on-time, fully-paid deadline produces zero payment penalties.</remarks>
    [Fact]
    public void Calculate_No_Penalty_When_FullyPaid()
    {
        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        TaxObligationSchedule schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 1.0m, 1)
                    .WithPenalty(new PenaltyDefinition
                    {
                        Type = PenaltyType.Recouvrement,
                        PeriodRate = 0.10m
                    }));

        rule.ConfigureObligationSchedule(schedule);

        // Fully paid
        Dictionary<string, decimal> payments = new Dictionary<string, decimal> { { "PAY1", 1000m } };
        DateTimeOffset asOf = new DateTimeOffset(2025, 3, 2, 0, 0, 0, TimeSpan.Zero);
        ObligationPenaltyResult result = ObligationPenaltyCalculator.Default.Calculate(rule, 1000m, asOf, payments);

        Assert.Empty(result.PaymentPenalties);
        Assert.Equal(0m, result.TotalPaymentPenalty);
    }

    /// <summary>
    /// Validates that penalties are produced for each overdue payment deadline in a schedule.
    /// </summary>
    /// <remarks>Ensures multiple overdue payment deadlines each generate their respective penalty entries.</remarks>
    [Fact]
    public void Calculate_Multiple_Payment_Deadlines()
    {
        TaxRule rule = new TaxRule { Key = "TAX1", Label = "Tax Rule 1" };
        TaxObligationSchedule schedule = TaxObligationSchedule.Create()
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 1)
                    .WithPenalty(new PenaltyDefinition { Type = PenaltyType.Recouvrement, PeriodRate = 0.05m, Period = Duration.Days(30) }))
            .AddPaymentDeadline(
                PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 2)
                    .WithPenalty(new PenaltyDefinition { Type = PenaltyType.Recouvrement, PeriodRate = 0.10m, Period = Duration.Days(30) }));

        rule.ConfigureObligationSchedule(schedule);

        // Both deadlines overdue
        DateTimeOffset asOf = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero);
        ObligationPenaltyResult result = ObligationPenaltyCalculator.Default.Calculate(rule, 10000m, asOf);

        Assert.True(result.PaymentPenalties.ContainsKey("PAY1"));
        Assert.True(result.PaymentPenalties.ContainsKey("PAY2"));
    }
}
