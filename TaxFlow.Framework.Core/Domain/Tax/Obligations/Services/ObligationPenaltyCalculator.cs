using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Penalties;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Result of obligation penalty calculation.
/// </summary>
public sealed class ObligationPenaltyResult
{
    /// <summary>
    /// Declaration penalty accruals grouped by declaration deadline key.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<PenaltyAccrual>> DeclarationPenaltiesByKey { get; init; } =
        new Dictionary<string, IReadOnlyList<PenaltyAccrual>>();

    /// <summary>
    /// All declaration penalty accruals combined (for backward compatibility).
    /// </summary>
    public IReadOnlyList<PenaltyAccrual> DeclarationPenalties =>
        DeclarationPenaltiesByKey.Values.SelectMany(p => p).ToList().AsReadOnly();

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
            {
                all.AddRange(penalties);
            }

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

    /// <summary>
    /// Gets penalties for a specific declaration deadline.
    /// </summary>
    public IReadOnlyList<PenaltyAccrual> GetDeclarationPenalties(string declarationKey)
    {
        return DeclarationPenaltiesByKey.TryGetValue(declarationKey, out var penalties)
            ? penalties
            : [];
    }

    /// <summary>
    /// Gets penalties for a specific payment deadline.
    /// </summary>
    public IReadOnlyList<PenaltyAccrual> GetPaymentPenalties(string paymentKey)
    {
        return PaymentPenalties.TryGetValue(paymentKey, out var penalties)
            ? penalties
            : [];
    }
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

        return rule.ObligationSchedule is null
            ? new ObligationPenaltyResult()
            : Calculate(rule.ObligationSchedule, taxAmount, asOf, payments);
    }

    /// <summary>
    /// Calculates penalties for an obligation schedule.
    /// </summary>
    /// <param name="schedule">The obligation schedule.</param>
    /// <param name="taxAmount">The calculated tax amount.</param>
    /// <param name="asOf">Date for penalty calculation.</param>
    /// <param name="payments">Optional dictionary of payments made keyed by deadline key.</param>
    public ObligationPenaltyResult Calculate(
        TaxObligationSchedule schedule,
        decimal taxAmount,
        DateTimeOffset asOf,
        IReadOnlyDictionary<string, decimal>? payments = null)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        payments ??= new Dictionary<string, decimal>();

        Dictionary<string, IReadOnlyList<PenaltyAccrual>> declarationPenalties = CalculateAllDeclarationPenalties(schedule.DeclarationDeadlines, taxAmount, asOf);
        Dictionary<string, IReadOnlyList<PenaltyAccrual>> paymentPenalties = CalculatePaymentPenalties(schedule.PaymentDeadlines, taxAmount, asOf, payments);

        return new ObligationPenaltyResult
        {
            DeclarationPenaltiesByKey = declarationPenalties,
            PaymentPenalties = paymentPenalties
        };
    }

    private Dictionary<string, IReadOnlyList<PenaltyAccrual>> CalculateAllDeclarationPenalties(
        IReadOnlyList<DeclarationDeadline> deadlines,
        decimal taxAmount,
        DateTimeOffset asOf)
    {
        Dictionary<string, IReadOnlyList<PenaltyAccrual>> result = new();

        foreach (var deadline in deadlines)
        {
            List<PenaltyAccrual> penalties = CalculateDeclarationPenalties(deadline, taxAmount, asOf);
            if (penalties.Count > 0)
            {
                result[deadline.Key] = penalties.AsReadOnly();
            }
        }

        return result;
    }

    private List<PenaltyAccrual> CalculateDeclarationPenalties(
        DeclarationDeadline deadline,
        decimal taxAmount,
        DateTimeOffset asOf)
    {
        List<PenaltyAccrual> penalties = new();

        if (deadline.PenaltyDefinition is null || !deadline.IsOverdue(asOf))
        {
            return penalties;
        }

        PenaltyDefinition definition = deadline.PenaltyDefinition;
        int daysLate = deadline.GetDaysLate(asOf);
        Guid declarationId = Guid.NewGuid();

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
            int periodDaysApprox = definition.Period.ToDays();
            int periodCount = PenaltyCalculationHelper.CalculatePeriodCount(daysLate, periodDaysApprox);
            decimal baseAmount = taxAmount;

            for (var period = 1; period <= periodCount; period++)
            {
                DateTimeOffset periodStart = definition.GetPeriodStartDate(deadline.EffectiveDueDate, period);
                DateTimeOffset periodEnd = definition.GetPeriodEndDate(deadline.EffectiveDueDate, period);
                DateTimeOffset cappedEnd = periodEnd < asOf ? periodEnd : asOf;
                int daysInPeriod = (int)Math.Max(0, (cappedEnd.Date - periodStart.Date).TotalDays);

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
