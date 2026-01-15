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

        var policy = new PenaltyPolicy
        {
            AssietteFixedAmount = 100m,
            AssietteAnnualRate = 0.12m,
            DaysInYear = 360,
            AssietteGraceDays = 10,
            AssiettePeriodDays = 30
        };

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
    public void Calculate_Recouvrement_Periodic_Lines_On_Unpaid()
    {
        var declarationId = Guid.NewGuid();
        var installmentId = Guid.NewGuid();
        var installment = new Installment(installmentId, 1000m, new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var schedule = new PaymentSchedule(declarationId, null, new[] { installment });

        var payment = new Payment(Guid.NewGuid(), 400m, new DateTimeOffset(2025, 1, 10, 0, 0, 0, TimeSpan.Zero));
        schedule.ApplyPayment(payment);

        var policy = new PenaltyPolicy
        {
            RecouvrementAnnualRate = 0.12m,
            DaysInYear = 360,
            RecouvrementGraceDays = 5,
            RecouvrementPeriodDays = 30
        };

        var asOf = new DateTimeOffset(2025, 2, 20, 0, 0, 0, TimeSpan.Zero);
        var result = PenaltyCalculator.Calculate(schedule, policy, asOf, taxBaseAmount: 1000m);

        var recLines = result.Accruals.Where(a => a.LineType == PenaltyLineType.RecouvrementRate).ToList();
        Assert.Equal(2, recLines.Count); // 45 days late => 2 periods of 30 days
        Assert.All(recLines, l => Assert.Equal(6m, l.Amount)); // 600 * 0.12 * 30 / 360
        Assert.All(recLines, l => Assert.Equal(installmentId, l.InstallmentId));
    }
}
