using System;
using System.Linq;

using Core.Domain.Tax.Penalties;
using Core.Domain.Tax.Penalties.Services;
using Core.Domain.Tax.Payments;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

/// <summary>
/// Unit tests for the PenaltyCalculator covering declaration and payment penalty scenarios.
/// </summary>
/// <remarks>Scenarios include fixed penalties, prorated installments, outstanding balances, and rate increments.</remarks>
public class PenaltyCalculatorTests
{
    /// <summary>
    /// Validates that both fixed and periodic assiette penalties are generated for late declarations.
    /// </summary>
    /// <remarks>Combines fixed amount penalties with rate-based periods to verify multiple line types.</remarks>
    [Fact]
    public void Calculate_Assiette_Fixed_And_Periodic_Rate_Lines()
    {
        Guid declarationId = Guid.NewGuid();
        Installment[] installments =
        [
            new Installment(Guid.NewGuid(), 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
        ];
        PaymentSchedule schedule = new PaymentSchedule(declarationId, null, installments);

        PenaltyPolicy policy = new PenaltyPolicy { DaysInYear = 360 };
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Assiette,
            FixedAmount = 100m,
            AnnualRate = 0.12m,
            GracePeriod = Duration.Days(10),
            Period = Duration.Days(30)
        });

        DateTimeOffset asOf = new DateTimeOffset(2025, 2, 20, 0, 0, 0, TimeSpan.Zero);
        PenaltyCalculationResult result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        PenaltyAccrual fixedLine = result.Accruals.Single(a => a.LineType == PenaltyLineType.AssietteFixed);
        Assert.Equal(100m, fixedLine.Amount);
        Assert.Equal(declarationId, fixedLine.DeclarationId);

        // Jan 1 + 10 grace = Jan 11 effective due
        // Jan 11 to Feb 20 = 40 days late
        // Period 1: Jan 11 - Feb 10 = 30 days -> 1000 * 0.12 * 30 / 360 = 10m
        // Period 2: Feb 10 - Feb 20 = 10 days -> 1000 * 0.12 * 10 / 360 = 3.33m
        List<PenaltyAccrual> rateLines = [.. result.Accruals.Where(a => a.LineType == PenaltyLineType.AssietteRate)];
        Assert.Equal(2, rateLines.Count);
        Assert.Equal(10m, rateLines[0].Amount);
        Assert.Equal(3.33m, Math.Round(rateLines[1].Amount, 2));
    }

    /// <summary>
    /// Ensures the last period of an assiette penalty is prorated when the late duration is partial.
    /// </summary>
    /// <remarks>Late period spans 40 days leading to one full period and one fractional period.</remarks>
    [Fact]
    public void Calculate_Assiette_Prorata_Last_Period()
    {
        Guid declarationId = Guid.NewGuid();
        Installment[] installments = new[]
        {
            new Installment(Guid.NewGuid(), 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
        };
        PaymentSchedule schedule = new PaymentSchedule(declarationId, null, installments);

        PenaltyPolicy policy = new PenaltyPolicy { DaysInYear = 360 };
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Assiette,
            AnnualRate = 0.12m,
            GracePeriod = Duration.Zero,
            Period = Duration.Days(30)
        });

        DateTimeOffset asOf = new DateTimeOffset(2025, 2, 10, 0, 0, 0, TimeSpan.Zero); // 40 days late => 30 + 10
        PenaltyCalculationResult result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        List<PenaltyAccrual> rateLines = [.. result.Accruals.Where(a => a.LineType == PenaltyLineType.AssietteRate)];
        Assert.Equal(2, rateLines.Count);
        Assert.Equal(10m, rateLines[0].Amount);
        Assert.Equal(3.33m, Math.Round(rateLines[1].Amount, 2));
    }

    /// <summary>
    /// Confirms recouvrement penalties accrue on unpaid balances after partial payments.
    /// </summary>
    /// <remarks>Applies a payment before calculation to ensure penalties target the remaining amount.</remarks>
    [Fact]
    public void Calculate_Recouvrement_Periodic_Lines_On_Unpaid()
    {
        Guid declarationId = Guid.NewGuid();
        Guid installmentId = Guid.NewGuid();
        Installment installment = new Installment(installmentId, 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        PaymentSchedule schedule = new PaymentSchedule(declarationId, null, [installment]);

        Payment payment = new Payment(Guid.NewGuid(), 400m, new DateTimeOffset(2025, 1, 10, 0, 0, 0, TimeSpan.Zero));
        schedule.ApplyPayment(payment);

        PenaltyPolicy policy = new PenaltyPolicy { DaysInYear = 360 };
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Recouvrement,
            AnnualRate = 0.12m,
            GracePeriod = Duration.Days(5),
            Period = Duration.Days(30)
        });

        DateTimeOffset asOf = new DateTimeOffset(2025, 2, 20, 0, 0, 0, TimeSpan.Zero);
        PenaltyCalculationResult result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        // Jan 1 + 5 grace = Jan 6 effective due
        // Jan 6 to Feb 20 = 45 days late
        // Period 1: Jan 6 - Feb 5 = 30 days -> 600 * 0.12 * 30 / 360 = 6m
        // Period 2: Feb 5 - Feb 20 = 15 days -> 600 * 0.12 * 15 / 360 = 3m
        List<PenaltyAccrual> recLines = [.. result.Accruals.Where(a => a.LineType == PenaltyLineType.RecouvrementRate)];
        Assert.Equal(2, recLines.Count);
        Assert.Equal(6m, recLines[0].Amount);
        Assert.Equal(3m, recLines[1].Amount);
        Assert.All(recLines, l => Assert.Equal(installmentId, l.InstallmentId));
    }

    /// <summary>
    /// Verifies that recouvrement penalties honor grace periods and apply incremental period rates.
    /// </summary>
    /// <remarks>Checks that successive periods use increasing rates after grace expiration.</remarks>
    [Fact]
    public void Calculate_Recouvrement_PeriodicRate_With_Increment_And_Grace()
    {
        Guid declarationId = Guid.NewGuid();
        Guid installmentId = Guid.NewGuid();
        Installment installment = new Installment(installmentId, 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        PaymentSchedule schedule = new PaymentSchedule(declarationId, null, [installment]);

        PenaltyPolicy policy = new PenaltyPolicy();
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Recouvrement,
            GracePeriod = Duration.Days(10),
            Period = Duration.Days(30),
            PeriodRate = 0.10m,
            PeriodRateIncrement = 0.01m
        });

        DateTimeOffset asOf = new DateTimeOffset(2025, 3, 15, 0, 0, 0, TimeSpan.Zero);
        PenaltyCalculationResult result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        // From Jan 11 to Mar 15 => 63 days late => 3 periods
        List<PenaltyAccrual> recLines = [.. result.Accruals.Where(a => a.LineType == PenaltyLineType.RecouvrementRate)];
        Assert.Equal(3, recLines.Count);

        // Period rates: 10%, 11%, 12%
        Assert.Equal(100m, recLines[0].Amount);
        Assert.Equal(110m, recLines[1].Amount);
        Assert.Equal(120m, recLines[2].Amount);
    }

    /// <summary>
    /// Ensures outstanding amounts are recalculated per period when payments occur mid-schedule.
    /// </summary>
    /// <remarks>Tests that a payment between periods reduces subsequent penalty amounts.</remarks>
    [Fact]
    public void Calculate_Recouvrement_Uses_Outstanding_Per_Period()
    {
        Guid declarationId = Guid.NewGuid();
        Guid installmentId = Guid.NewGuid();
        Installment installment = new Installment(installmentId, 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        PaymentSchedule schedule = new PaymentSchedule(declarationId, null, [installment]);

        // Late payment reduces outstanding before second period
        Payment payment = new Payment(Guid.NewGuid(), 500m, new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero));
        schedule.ApplyPayment(payment);

        PenaltyPolicy policy = new PenaltyPolicy();
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Recouvrement,
            PeriodRate = 0.10m,
            GracePeriod = Duration.Zero,
            Period = Duration.Days(30)
        });

        // Jan 1 to Mar 5 = 63 days late => 3 periods
        // Period 1: Jan 1 - Jan 31 = 30 days, outstanding = 1000 -> 100m
        // Period 2: Jan 31 - Mar 1 = 30 days, outstanding = 500 (paid Feb 1) -> 50m
        // Period 3: Mar 1 - Mar 5 = partial period
        DateTimeOffset asOf = new DateTimeOffset(2025, 3, 5, 0, 0, 0, TimeSpan.Zero);
        PenaltyCalculationResult result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        List<PenaltyAccrual> recLines = [.. result.Accruals.Where(a => a.LineType == PenaltyLineType.RecouvrementRate)];
        Assert.True(recLines.Count >= 2); // At least two periods
        Assert.Equal(100m, recLines[0].Amount); // 10% of 1000
        Assert.Equal(50m, recLines[1].Amount);  // 10% of 500 after payment
    }
}
