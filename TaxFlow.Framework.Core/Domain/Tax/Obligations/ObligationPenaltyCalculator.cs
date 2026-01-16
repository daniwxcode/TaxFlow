using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Penalties;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Result of obligation penalty calculation.
/// </summary>
public sealed class ObligationPenaltyResult
{
    /// <summary>
    /// Declaration penalty accruals (if any).
    /// </summary>
    public IReadOnlyList<PenaltyAccrual> DeclarationPenalties { get; init; } = [];

    /// <summary>
    /// Payment penalty accruals grouped by payment deadline key.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<PenaltyAccrual>> PaymentPenalties { get; init; } =
        new Dictionary<string, IReadOnlyList<PenaltyAccrual>>();

    /// <summary>
    /// All penalty accruals combined.
    /// </summary>
    public IReadOnlyList<PenaltyAccrual> AllPenalties
    {
        get
        {
            var all = new List<PenaltyAccrual>(DeclarationPenalties);
            foreach (var penalties in PaymentPenalties.Values)
                all.AddRange(penalties);
            return all.AsReadOnly();
        }
    }

    /// <summary>
    /// Total penalty amount.
    /// </summary>
    public decimal TotalAmount => AllPenalties.Sum(p => p.Amount);

    /// <summary>
    /// Total declaration penalty amount.
    /// </summary>
    public decimal TotalDeclarationPenalty => DeclarationPenalties.Sum(p => p.Amount);

    /// <summary>
    /// Total payment penalty amount.
    /// </summary>
    public decimal TotalPaymentPenalty => PaymentPenalties.Values.SelectMany(p => p).Sum(p => p.Amount);
}

/// <summary>
/// Calculates penalties based on tax obligation schedules.
/// </summary>
public sealed class ObligationPenaltyCalculator
{
    private readonly PenaltyPolicy _defaultPolicy;

    /// <summary>
    /// Creates a new obligation penalty calculator.
    /// </summary>
    /// <param name="defaultPolicy">Default policy for calculations when deadlines don't specify one.</param>
    public ObligationPenaltyCalculator(PenaltyPolicy? defaultPolicy = null)
    {
        _defaultPolicy = defaultPolicy ?? new PenaltyPolicy();
    }

    /// <summary>
    /// Singleton instance with default policy.
    /// </summary>
    public static ObligationPenaltyCalculator Default { get; } = new();

    /// <summary>
    /// Calculates penalties for a tax rule's obligation schedule.
    /// </summary>
    /// <param name="rule">The tax rule with obligation schedule.</param>
    /// <param name="taxAmount">The calculated tax amount.</param>
    /// <param name="asOf">Date for penalty calculation.</param>
    /// <param name="payments">Optional dictionary of payments made keyed by deadline key.</param>
    public ObligationPenaltyResult Calculate(
        TaxRule rule,
        decimal taxAmount,
        DateTimeOffset asOf,
        IReadOnlyDictionary<string, decimal>? payments = null)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (rule.ObligationSchedule is null)
            return new ObligationPenaltyResult();

        var schedule = rule.ObligationSchedule;
        payments ??= new Dictionary<string, decimal>();

        var declarationPenalties = CalculateDeclarationPenalties(schedule.DeclarationDeadline, taxAmount, asOf);
        var paymentPenalties = CalculatePaymentPenalties(schedule.PaymentDeadlines, taxAmount, asOf, payments);

        return new ObligationPenaltyResult
        {
            DeclarationPenalties = declarationPenalties,
            PaymentPenalties = paymentPenalties
        };
    }

    private List<PenaltyAccrual> CalculateDeclarationPenalties(
        DeclarationDeadline? deadline,
        decimal taxAmount,
        DateTimeOffset asOf)
    {
        var penalties = new List<PenaltyAccrual>();

        if (deadline?.PenaltyDefinition is null || !deadline.IsOverdue(asOf))
            return penalties;

        var definition = deadline.PenaltyDefinition;
        var daysLate = deadline.GetDaysLate(asOf);
        var declarationId = Guid.NewGuid();

        // Fixed penalty
        if (definition.FixedAmount > 0)
        {
            var amount = PenaltyCalculationHelper.ApplyFloorAndCap(
                definition.FixedAmount,
                definition.Minimum,
                definition.Cap);

            penalties.Add(new PenaltyAccrual(
                PenaltyType.Assiette,
                PenaltyLineType.AssietteFixed,
                declarationId,
                liquidationId: null,
                installmentId: null,
                taxAmount,
                amount,
                rate: 0m,
                daysLate,
                periodIndex: 0,
                periodDays: 0,
                deadline.DueDate,
                deadline.DueDate,
                asOf));
        }

        // Periodic penalties
        if (definition.AnnualRate > 0)
        {
            var periodDaysApprox = definition.Period.ToDays();
            var periodCount = PenaltyCalculationHelper.CalculatePeriodCount(daysLate, periodDaysApprox);
            var baseAmount = taxAmount;

            for (var period = 1; period <= periodCount; period++)
            {
                var periodStart = definition.GetPeriodStartDate(deadline.EffectiveDueDate, period);
                var periodEnd = definition.GetPeriodEndDate(deadline.EffectiveDueDate, period);
                var cappedEnd = periodEnd < asOf ? periodEnd : asOf;
                var daysInPeriod = (int)Math.Max(0, (cappedEnd.Date - periodStart.Date).TotalDays);

                if (daysInPeriod == 0)
                    continue;

                var amount = PenaltyCalculationHelper.Prorate(
                    baseAmount,
                    definition.AnnualRate,
                    _defaultPolicy.DaysInYear,
                    daysInPeriod);

                amount = PenaltyCalculationHelper.ApplyFloorAndCap(amount, definition.Minimum, definition.Cap);

                if (amount < _defaultPolicy.MinimumLineAmount)
                    continue;

                penalties.Add(new PenaltyAccrual(
                    PenaltyType.Assiette,
                    PenaltyLineType.AssietteRate,
                    declarationId,
                    liquidationId: null,
                    installmentId: null,
                    baseAmount,
                    amount,
                    definition.AnnualRate,
                    daysLate,
                    period,
                    daysInPeriod,
                    periodStart,
                    cappedEnd,
                    asOf));

                if (definition.Capitalize)
                    baseAmount += amount;
            }
        }

        return penalties;
    }

    private Dictionary<string, IReadOnlyList<PenaltyAccrual>> CalculatePaymentPenalties(
        IReadOnlyList<PaymentDeadline> deadlines,
        decimal totalTaxAmount,
        DateTimeOffset asOf,
        IReadOnlyDictionary<string, decimal> payments)
    {
        var result = new Dictionary<string, IReadOnlyList<PenaltyAccrual>>();

        foreach (var deadline in deadlines)
        {
            if (deadline.PenaltyDefinition is null || !deadline.IsOverdue(asOf))
                continue;

            var amountDue = deadline.GetAmountDue(totalTaxAmount);
            var amountPaid = payments.TryGetValue(deadline.Key, out var paid) ? paid : 0m;
            var outstanding = amountDue - amountPaid;

            if (outstanding <= 0)
                continue;

            var penalties = CalculatePaymentDeadlinePenalties(deadline, outstanding, asOf);
            if (penalties.Count > 0)
                result[deadline.Key] = penalties.AsReadOnly();
        }

        return result;
    }

    private List<PenaltyAccrual> CalculatePaymentDeadlinePenalties(
        PaymentDeadline deadline,
        decimal outstanding,
        DateTimeOffset asOf)
    {
        var penalties = new List<PenaltyAccrual>();
        var definition = deadline.PenaltyDefinition!;
        var daysLate = deadline.GetDaysLate(asOf);
        var installmentId = Guid.NewGuid();
        var declarationId = Guid.NewGuid();

        var periodDaysApprox = definition.Period.ToDays();
        var periodCount = PenaltyCalculationHelper.CalculatePeriodCount(daysLate, periodDaysApprox);
        var accumulatedPenalty = 0m;

        for (var period = 1; period <= periodCount; period++)
        {
            var periodStart = definition.GetPeriodStartDate(deadline.EffectiveDueDate, period);
            var periodEnd = definition.GetPeriodEndDate(deadline.EffectiveDueDate, period);
            var cappedEnd = periodEnd < asOf ? periodEnd : asOf;
            var daysInPeriod = (int)Math.Max(0, (cappedEnd.Date - periodStart.Date).TotalDays);

            if (daysInPeriod == 0)
                continue;

            var baseAmount = definition.Capitalize ? outstanding + accumulatedPenalty : outstanding;

            var rate = definition.PeriodRate > 0
                ? definition.PeriodRate + (period - 1) * definition.PeriodRateIncrement
                : definition.AnnualRate;

            var penalty = definition.PeriodRate > 0
                ? baseAmount * rate
                : PenaltyCalculationHelper.Prorate(baseAmount, rate, _defaultPolicy.DaysInYear, daysInPeriod);

            penalty = PenaltyCalculationHelper.ApplyFloorAndCap(penalty, definition.Minimum, definition.Cap);

            if (penalty < _defaultPolicy.MinimumLineAmount)
                continue;

            penalties.Add(new PenaltyAccrual(
                PenaltyType.Recouvrement,
                PenaltyLineType.RecouvrementRate,
                declarationId,
                liquidationId: null,
                installmentId,
                baseAmount,
                penalty,
                rate,
                daysLate,
                period,
                daysInPeriod,
                periodStart,
                cappedEnd,
                asOf));

            if (definition.Capitalize)
                accumulatedPenalty += penalty;
        }

        return penalties;
    }
}
