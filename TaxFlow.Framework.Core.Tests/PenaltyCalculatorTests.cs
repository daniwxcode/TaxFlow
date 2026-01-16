using System;
using System.Linq;

using Core.Domain.Tax;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class PenaltyCalculatorTests
{
    [Fact]
    public void Calculate_Assiette_Fixed_And_Periodic_Rate_Lines()
    {
        var declarationId = Guid.NewGuid();
        var installments = new[]
        {
            new Installment(Guid.NewGuid(), 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
        };
        var schedule = new PaymentSchedule(declarationId, null, installments);

        var policy = new PenaltyPolicy { DaysInYear = 360 };
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Assiette,
            FixedAmount = 100m,
            AnnualRate = 0.12m,
            GraceDays = 10,
            PeriodDays = 30
        });

        var asOf = new DateTimeOffset(2025, 2, 20, 0, 0, 0, TimeSpan.Zero);
        var result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        var fixedLine = result.Accruals.Single(a => a.LineType == PenaltyLineType.AssietteFixed);
        Assert.Equal(100m, fixedLine.Amount);
        Assert.Equal(declarationId, fixedLine.DeclarationId);

        var rateLines = result.Accruals.Where(a => a.LineType == PenaltyLineType.AssietteRate).ToList();
        Assert.Equal(2, rateLines.Count); // 40 days late => 2 periods of 30 days
        Assert.All(rateLines, l => Assert.Equal(10m, l.Amount)); // 1000 * 0.12 * 30 / 360
    }

    [Fact]
    public void Calculate_Assiette_Prorata_Last_Period()
    {
        var declarationId = Guid.NewGuid();
        var installments = new[]
        {
            new Installment(Guid.NewGuid(), 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
        };
        var schedule = new PaymentSchedule(declarationId, null, installments);

        var policy = new PenaltyPolicy { DaysInYear = 360 };
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Assiette,
            AnnualRate = 0.12m,
            GraceDays = 0,
            PeriodDays = 30
        });

        var asOf = new DateTimeOffset(2025, 2, 10, 0, 0, 0, TimeSpan.Zero); // 40 days late => 30 + 10
        var result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        var rateLines = result.Accruals.Where(a => a.LineType == PenaltyLineType.AssietteRate).ToList();
        Assert.Equal(2, rateLines.Count);
        Assert.Equal(10m, rateLines[0].Amount);
        Assert.Equal(3.33m, Math.Round(rateLines[1].Amount, 2));
    }

    [Fact]
    public void Calculate_Recouvrement_Periodic_Lines_On_Unpaid()
    {
        var declarationId = Guid.NewGuid();
        var installmentId = Guid.NewGuid();
        var installment = new Installment(installmentId, 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var schedule = new PaymentSchedule(declarationId, null, new[] { installment });

        var payment = new Payment(Guid.NewGuid(), 400m, new DateTimeOffset(2025, 1, 10, 0, 0, 0, TimeSpan.Zero));
        schedule.ApplyPayment(payment);

        var policy = new PenaltyPolicy { DaysInYear = 360 };
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Recouvrement,
            AnnualRate = 0.12m,
            GraceDays = 5,
            PeriodDays = 30
        });

        var asOf = new DateTimeOffset(2025, 2, 20, 0, 0, 0, TimeSpan.Zero);
        var result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        var recLines = result.Accruals.Where(a => a.LineType == PenaltyLineType.RecouvrementRate).ToList();
        Assert.Equal(2, recLines.Count); // 45 days late => 2 periods of 30 days
        Assert.All(recLines, l => Assert.Equal(6m, l.Amount)); // 600 * 0.12 * 30 / 360
        Assert.All(recLines, l => Assert.Equal(installmentId, l.InstallmentId));
    }

    [Fact]
    public void Calculate_Recouvrement_PeriodicRate_With_Increment_And_Grace()
    {
        var declarationId = Guid.NewGuid();
        var installmentId = Guid.NewGuid();
        var installment = new Installment(installmentId, 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var schedule = new PaymentSchedule(declarationId, null, new[] { installment });

        var policy = new PenaltyPolicy();
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Recouvrement,
            GraceDays = 10,
            PeriodDays = 30,
            PeriodRate = 0.10m,
            PeriodRateIncrement = 0.01m
        });

        var asOf = new DateTimeOffset(2025, 3, 15, 0, 0, 0, TimeSpan.Zero);
        var result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        // From Jan 11 to Mar 15 => 63 days late => 3 periods
        var recLines = result.Accruals.Where(a => a.LineType == PenaltyLineType.RecouvrementRate).ToList();
        Assert.Equal(3, recLines.Count);

        // Period rates: 10%, 11%, 12%
        Assert.Equal(100m, recLines[0].Amount);
        Assert.Equal(110m, recLines[1].Amount);
        Assert.Equal(120m, recLines[2].Amount);
    }

    [Fact]
    public void Calculate_Recouvrement_Uses_Outstanding_Per_Period()
    {
        var declarationId = Guid.NewGuid();
        var installmentId = Guid.NewGuid();
        var installment = new Installment(installmentId, 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var schedule = new PaymentSchedule(declarationId, null, new[] { installment });

        // Late payment reduces outstanding before second period
        var payment = new Payment(Guid.NewGuid(), 500m, new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero));
        schedule.ApplyPayment(payment);

        var policy = new PenaltyPolicy();
        policy.AddOrUpdateDefinition(new PenaltyDefinition
        {
            Type = PenaltyType.Recouvrement,
            PeriodRate = 0.10m,
            GraceDays = 0,
            PeriodDays = 30
        });

        var asOf = new DateTimeOffset(2025, 3, 5, 0, 0, 0, TimeSpan.Zero);
        var result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        var recLines = result.Accruals.Where(a => a.LineType == PenaltyLineType.RecouvrementRate).ToList();
        Assert.Equal(2, recLines.Count); // two periods
        Assert.Equal(100m, recLines[0].Amount); // 10% of 1000
        Assert.Equal(50m, recLines[1].Amount);  // 10% of 500 after payment
    }
}
