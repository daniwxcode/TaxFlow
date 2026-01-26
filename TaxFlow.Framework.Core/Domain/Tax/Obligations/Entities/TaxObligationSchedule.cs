using Core.Domain.Contracts.Abstracts;
using Core.Domain.Contracts.Validation;
using Core.Domain.Localization;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Represents the complete schedule of obligations for a tax rule,
/// including declaration deadlines and payment deadlines.
/// Supports multiple declaration and payment deadlines per tax.
/// </summary>
public sealed class TaxObligationSchedule : AuditableEntity
{
    private readonly List<DeclarationDeadline> _declarationDeadlines = [];
    private readonly List<PaymentDeadline> _paymentDeadlines = [];
    private readonly List<LegalReference> _legalReferences = [];

    /// <summary>
    /// Declaration deadlines ordered by their due date.
    /// </summary>
    public IReadOnlyList<DeclarationDeadline> DeclarationDeadlines =>
        _declarationDeadlines.OrderBy(d => d.Order).ThenBy(d => d.DueDate).ToList().AsReadOnly();

    /// <summary>
    /// Gets the primary declaration deadline (first in order).
    /// For backward compatibility with single-declaration scenarios.
    /// </summary>
    public DeclarationDeadline? DeclarationDeadline =>
        _declarationDeadlines.OrderBy(d => d.Order).ThenBy(d => d.DueDate).FirstOrDefault();

    /// <summary>
    /// Payment deadlines ordered by their due date.
    /// </summary>
    public IReadOnlyList<PaymentDeadline> PaymentDeadlines =>
        _paymentDeadlines.OrderBy(p => p.Order).ThenBy(p => p.DueDate).ToList().AsReadOnly();

    /// <summary>
    /// Legal references that apply to this entire schedule.
    /// </summary>
    public IReadOnlyList<LegalReference> LegalReferences => _legalReferences.AsReadOnly();

    /// <summary>
    /// Total number of declaration deadlines.
    /// </summary>
    public int DeclarationCount => _declarationDeadlines.Count;

    /// <summary>
    /// Total number of payment installments.
    /// </summary>
    public int InstallmentCount => _paymentDeadlines.Count;

    /// <summary>
    /// Whether this schedule has at least one declaration deadline.
    /// </summary>
    public bool HasDeclarationDeadline => _declarationDeadlines.Count > 0;

    /// <summary>
    /// Whether this schedule has multiple declaration deadlines.
    /// </summary>
    public bool HasMultipleDeclarations => _declarationDeadlines.Count > 1;

    /// <summary>
    /// Whether this schedule has any payment deadlines.
    /// </summary>
    public bool HasPaymentDeadlines => _paymentDeadlines.Count > 0;

    /// <summary>
    /// Whether this schedule has advance and balance payments.
    /// </summary>
    public bool HasFractionalPayments =>
        _paymentDeadlines.Any(p => p.PaymentType == PaymentType.Advance) &&
        _paymentDeadlines.Any(p => p.PaymentType == PaymentType.Balance);

    /// <summary>
    /// Name/label for this schedule.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Description of this schedule.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Fiscal year this schedule applies to.
    /// </summary>
    public int? FiscalYear { get; init; }

    /// <summary>
    /// Creates an empty obligation schedule.
    /// </summary>
    public static TaxObligationSchedule Create(string? name = null, int? fiscalYear = null) =>
        new()
        {
            Name = name,
            FiscalYear = fiscalYear
        };

    /// <summary>
    /// Sets the declaration deadline for this schedule (single declaration).
    /// For backward compatibility - clears existing declarations and adds this one.
    /// </summary>
    /// <param name="deadline">Declaration deadline.</param>
    public TaxObligationSchedule WithDeclarationDeadline(DeclarationDeadline deadline)
    {
        ArgumentNullException.ThrowIfNull(deadline);
        _declarationDeadlines.Clear();
        _declarationDeadlines.Add(deadline);
        return this;
    }

    /// <summary>
    /// Adds a declaration deadline to this schedule.
    /// </summary>
    /// <param name="deadline">Declaration deadline to add.</param>
    public TaxObligationSchedule AddDeclarationDeadline(DeclarationDeadline deadline)
    {
        ArgumentNullException.ThrowIfNull(deadline);

        if (_declarationDeadlines.Any(d => d.Key.Equals(deadline.Key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(ExceptionMessages.DeclarationDeadlineAlreadyExists.Format(("key", deadline.Key)));
        }

        _declarationDeadlines.Add(deadline);
        return this;
    }

    /// <summary>
    /// Removes a declaration deadline by key.
    /// </summary>
    /// <param name="key">Key of the deadline to remove.</param>
    public bool RemoveDeclarationDeadline(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _declarationDeadlines.RemoveAll(d => d.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    /// <summary>
    /// Adds a payment deadline to this schedule.
    /// </summary>
    /// <param name="deadline">Payment deadline to add.</param>
    public TaxObligationSchedule AddPaymentDeadline(PaymentDeadline deadline)
    {
        ArgumentNullException.ThrowIfNull(deadline);

        if (_paymentDeadlines.Any(p => p.Key.Equals(deadline.Key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(ExceptionMessages.PaymentDeadlineAlreadyExists.Format(("key", deadline.Key)));
        }

        _paymentDeadlines.Add(deadline);
        return this;
    }

    /// <summary>
    /// Removes a payment deadline by key.
    /// </summary>
    /// <param name="key">Key of the deadline to remove.</param>
    public bool RemovePaymentDeadline(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _paymentDeadlines.RemoveAll(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    /// <summary>
    /// Clears all declaration deadlines.
    /// </summary>
    public TaxObligationSchedule ClearDeclarationDeadlines()
    {
        _declarationDeadlines.Clear();
        return this;
    }

    /// <summary>
    /// Clears the declaration deadline (backward compatibility alias).
    /// </summary>
    public TaxObligationSchedule ClearDeclarationDeadline() => ClearDeclarationDeadlines();

    /// <summary>
    /// Clears all payment deadlines.
    /// </summary>
    public TaxObligationSchedule ClearPaymentDeadlines()
    {
        _paymentDeadlines.Clear();
        return this;
    }

    /// <summary>
    /// Adds a legal reference to this schedule.
    /// </summary>
    public TaxObligationSchedule AddLegalReference(LegalReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        _legalReferences.Add(reference);
        return this;
    }

    /// <summary>
    /// Clears all legal references.
    /// </summary>
    public TaxObligationSchedule ClearLegalReferences()
    {
        _legalReferences.Clear();
        return this;
    }

    /// <summary>
    /// Gets a declaration deadline by key.
    /// </summary>
    public DeclarationDeadline? GetDeclarationDeadline(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _declarationDeadlines.FirstOrDefault(d => d.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets a payment deadline by key.
    /// </summary>
    public PaymentDeadline? GetPaymentDeadline(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _paymentDeadlines.FirstOrDefault(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets payment deadlines linked to a specific declaration.
    /// </summary>
    public IReadOnlyList<PaymentDeadline> GetPaymentsForDeclaration(string declarationKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(declarationKey);
        return _paymentDeadlines
            .Where(p => p.LinkedDeclarationKey?.Equals(declarationKey, StringComparison.OrdinalIgnoreCase) == true)
            .OrderBy(p => p.Order)
            .ThenBy(p => p.DueDate)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Gets all advance payments.
    /// </summary>
    public IReadOnlyList<PaymentDeadline> GetAdvancePayments() =>
        _paymentDeadlines.Where(p => p.PaymentType == PaymentType.Advance)
            .OrderBy(p => p.Order)
            .ThenBy(p => p.DueDate)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets all balance/final payments.
    /// </summary>
    public IReadOnlyList<PaymentDeadline> GetBalancePayments() =>
        _paymentDeadlines.Where(p => p.PaymentType == PaymentType.Balance)
            .OrderBy(p => p.Order)
            .ThenBy(p => p.DueDate)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets declarations for a specific regime.
    /// </summary>
    public IReadOnlyList<DeclarationDeadline> GetDeclarationsForRegime(TaxRegime regime) =>
        _declarationDeadlines.Where(d => d.Regime == regime)
            .OrderBy(d => d.Order)
            .ThenBy(d => d.DueDate)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets payments for a specific regime.
    /// </summary>
    public IReadOnlyList<PaymentDeadline> GetPaymentsForRegime(TaxRegime regime) =>
        _paymentDeadlines.Where(p => p.Regime == regime)
            .OrderBy(p => p.Order)
            .ThenBy(p => p.DueDate)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Validates the schedule configuration.
    /// </summary>
    public ValidationResult Validate()
    {
        var errors = new List<ValidationError>();

        // Validate unique keys across all declarations
        var duplicateDeclarationKeys = _declarationDeadlines
            .GroupBy(d => d.Key, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var key in duplicateDeclarationKeys)
        {
            errors.Add(new ValidationError(
                "DUPLICATE_DECLARATION_KEY",
                ValidationMessages.DuplicateDeclarationKey.Format(("key", key)),
                nameof(DeclarationDeadlines)));
        }

        // Validate payment fractions sum to 1.0 (or less if partial payments are allowed)
        if (_paymentDeadlines.Count > 0)
        {
            // Group by linked declaration and validate each group
            List<IGrouping<string, PaymentDeadline>> paymentGroups = _paymentDeadlines
                .GroupBy(p => p.LinkedDeclarationKey ?? "__unlinked__")
                .ToList();

            foreach (var group in paymentGroups)
            {
                decimal totalFraction = group.Sum(p => p.Fraction);
                if (totalFraction > 1.0m)
                {
                    errors.Add(new ValidationError(
                        "INVALID_FRACTION_TOTAL",
                        ValidationMessages.InvalidFractionTotal.Format(("total", totalFraction.ToString("P0"))),
                        nameof(PaymentDeadlines)));
                }
            }
        }

        // Validate that declaration deadlines come before their linked payments
        foreach (var payment in _paymentDeadlines.Where(p => !string.IsNullOrEmpty(p.LinkedDeclarationKey)))
        {
            DeclarationDeadline? linkedDeclaration = _declarationDeadlines
                .FirstOrDefault(d => d.Key.Equals(payment.LinkedDeclarationKey, StringComparison.OrdinalIgnoreCase));

            if (linkedDeclaration is null)
            {
                errors.Add(new ValidationError(
                    "INVALID_LINKED_DECLARATION",
                    ValidationMessages.InvalidLinkedDeclaration.Format(
                        ("paymentKey", payment.Key),
                        ("declarationKey", payment.LinkedDeclarationKey ?? "")),
                    nameof(PaymentDeadlines)));
            }
            else if (linkedDeclaration.DueDate > payment.DueDate)
            {
                errors.Add(new ValidationError(
                    "DECLARATION_AFTER_PAYMENT",
                    ValidationMessages.DeclarationAfterPayment.Format(
                        ("declarationKey", linkedDeclaration.Key),
                        ("paymentKey", payment.Key)),
                    nameof(DeclarationDeadlines)));
            }
        }

        // Validate unique orders within each type
        List<int> duplicateDeclarationOrders = _declarationDeadlines
            .GroupBy(d => d.Order)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var order in duplicateDeclarationOrders)
        {
            errors.Add(new ValidationError(
                "DUPLICATE_DECLARATION_ORDER",
                ValidationMessages.DuplicateOrder.Format(
                    ("deadlineType", ValidationMessages.GetDeadlineTypeName("declaration").GetValue()),
                    ("order", order)),
                nameof(DeclarationDeadlines)));
        }

        List<int> duplicatePaymentOrders = _paymentDeadlines
            .GroupBy(p => p.Order)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var order in duplicatePaymentOrders)
        {
            errors.Add(new ValidationError(
                "DUPLICATE_PAYMENT_ORDER",
                ValidationMessages.DuplicateOrder.Format(
                    ("deadlineType", ValidationMessages.GetDeadlineTypeName("payment").GetValue()),
                    ("order", order)),
                nameof(PaymentDeadlines)));
        }

        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }

    /// <summary>
    /// Gets all deadlines that are overdue as of the given date.
    /// </summary>
    public IReadOnlyList<TaxDeadline> GetOverdueDeadlines(DateTimeOffset asOf)
    {
        List<TaxDeadline> overdue = new();

        overdue.AddRange(_declarationDeadlines.Where(d => d.IsOverdue(asOf)));
        overdue.AddRange(_paymentDeadlines.Where(p => p.IsOverdue(asOf)));

        return overdue.AsReadOnly();
    }

    /// <summary>
    /// Gets overdue declaration deadlines.
    /// </summary>
    public IReadOnlyList<DeclarationDeadline> GetOverdueDeclarations(DateTimeOffset asOf) =>
        _declarationDeadlines.Where(d => d.IsOverdue(asOf)).ToList().AsReadOnly();

    /// <summary>
    /// Gets overdue payment deadlines.
    /// </summary>
    public IReadOnlyList<PaymentDeadline> GetOverduePayments(DateTimeOffset asOf) =>
        _paymentDeadlines.Where(p => p.IsOverdue(asOf)).ToList().AsReadOnly();

    /// <summary>
    /// Gets the next upcoming deadline as of the given date.
    /// </summary>
    public TaxDeadline? GetNextDeadline(DateTimeOffset asOf)
    {
        List<TaxDeadline> allDeadlines = new ();
        allDeadlines.AddRange(_declarationDeadlines);
        allDeadlines.AddRange(_paymentDeadlines);

        return allDeadlines
            .Where(d => d.Enabled && d.DueDate > asOf)
            .OrderBy(d => d.DueDate)
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets the next upcoming declaration deadline.
    /// </summary>
    public DeclarationDeadline? GetNextDeclarationDeadline(DateTimeOffset asOf) =>
        _declarationDeadlines
            .Where(d => d.Enabled && d.DueDate > asOf)
            .OrderBy(d => d.DueDate)
            .FirstOrDefault();

    /// <summary>
    /// Gets the next upcoming payment deadline.
    /// </summary>
    public PaymentDeadline? GetNextPaymentDeadline(DateTimeOffset asOf) =>
        _paymentDeadlines
            .Where(p => p.Enabled && p.DueDate > asOf)
            .OrderBy(p => p.DueDate)
            .FirstOrDefault();

    /// <summary>
    /// Gets a summary of all legal references for this schedule.
    /// </summary>
    public string GetLegalBasisSummary()
    {
        List<string> allReferences = new ();

        // Schedule-level references
        allReferences.AddRange(_legalReferences.Select(r => r.GetCitation()));

        // Deadline-level references
        foreach (var deadline in _declarationDeadlines.Where(d => d.HasLegalBasis))
        {
            allReferences.Add($"{deadline.Label}: {deadline.GetLegalBasisSummary()}");
        }

        foreach (var deadline in _paymentDeadlines.Where(p => p.HasLegalBasis))
        {
            allReferences.Add($"{deadline.Label}: {deadline.GetLegalBasisSummary()}");
        }

        return string.Join("; ", allReferences.Distinct());
    }

    /// <summary>
    /// Calculates the total amount due for all payment deadlines.
    /// </summary>
    public decimal GetTotalAmountDue(decimal taxBase) =>
        _paymentDeadlines.Sum(p => p.GetAmountDue(taxBase));
}
