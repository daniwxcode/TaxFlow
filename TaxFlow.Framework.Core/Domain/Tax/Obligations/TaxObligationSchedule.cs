using Core.Domain.Contracts.Abstracts;
using Core.Domain.Contracts.Validation;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Represents the complete schedule of obligations for a tax rule,
/// including declaration deadline and payment deadlines.
/// </summary>
public sealed class TaxObligationSchedule : AuditableEntity
{
    private DeclarationDeadline? _declarationDeadline;
    private readonly List<PaymentDeadline> _paymentDeadlines = [];

    /// <summary>
    /// Optional declaration deadline for this schedule.
    /// </summary>
    public DeclarationDeadline? DeclarationDeadline => _declarationDeadline;

    /// <summary>
    /// Payment deadlines ordered by their due date.
    /// </summary>
    public IReadOnlyList<PaymentDeadline> PaymentDeadlines =>
        _paymentDeadlines.OrderBy(p => p.Order).ThenBy(p => p.DueDate).ToList().AsReadOnly();

    /// <summary>
    /// Total number of payment installments.
    /// </summary>
    public int InstallmentCount => _paymentDeadlines.Count;

    /// <summary>
    /// Whether this schedule has a declaration deadline.
    /// </summary>
    public bool HasDeclarationDeadline => _declarationDeadline is not null;

    /// <summary>
    /// Whether this schedule has any payment deadlines.
    /// </summary>
    public bool HasPaymentDeadlines => _paymentDeadlines.Count > 0;

    /// <summary>
    /// Creates an empty obligation schedule.
    /// </summary>
    public static TaxObligationSchedule Create() => new();

    /// <summary>
    /// Sets the declaration deadline for this schedule.
    /// </summary>
    /// <param name="deadline">Declaration deadline.</param>
    public TaxObligationSchedule WithDeclarationDeadline(DeclarationDeadline deadline)
    {
        ArgumentNullException.ThrowIfNull(deadline);
        _declarationDeadline = deadline;
        return this;
    }

    /// <summary>
    /// Adds a payment deadline to this schedule.
    /// </summary>
    /// <param name="deadline">Payment deadline to add.</param>
    public TaxObligationSchedule AddPaymentDeadline(PaymentDeadline deadline)
    {
        ArgumentNullException.ThrowIfNull(deadline);

        if (_paymentDeadlines.Any(p => p.Key.Equals(deadline.Key, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A payment deadline with key '{deadline.Key}' already exists.");

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
    /// Clears the declaration deadline.
    /// </summary>
    public TaxObligationSchedule ClearDeclarationDeadline()
    {
        _declarationDeadline = null;
        return this;
    }

    /// <summary>
    /// Clears all payment deadlines.
    /// </summary>
    public TaxObligationSchedule ClearPaymentDeadlines()
    {
        _paymentDeadlines.Clear();
        return this;
    }

    /// <summary>
    /// Validates the schedule configuration.
    /// </summary>
    public ValidationResult Validate()
    {
        var errors = new List<ValidationError>();

        // Validate payment fractions sum to 1.0 (or less if partial payments are allowed)
        if (_paymentDeadlines.Count > 0)
        {
            var totalFraction = _paymentDeadlines.Sum(p => p.Fraction);
            if (totalFraction > 1.0m)
            {
                errors.Add(new ValidationError(
                    "INVALID_FRACTION_TOTAL",
                    $"Total payment fractions ({totalFraction:P0}) exceed 100%.",
                    nameof(PaymentDeadlines)));
            }
        }

        // Validate that declaration deadline comes before first payment
        if (_declarationDeadline is not null && _paymentDeadlines.Count > 0)
        {
            var firstPayment = _paymentDeadlines.MinBy(p => p.DueDate);
            if (firstPayment is not null && _declarationDeadline.DueDate > firstPayment.DueDate)
            {
                errors.Add(new ValidationError(
                    "DECLARATION_AFTER_PAYMENT",
                    "Declaration deadline must be before or equal to the first payment deadline.",
                    nameof(DeclarationDeadline)));
            }
        }

        // Validate unique orders
        var duplicateOrders = _paymentDeadlines
            .GroupBy(p => p.Order)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var order in duplicateOrders)
        {
            errors.Add(new ValidationError(
                "DUPLICATE_PAYMENT_ORDER",
                $"Multiple payment deadlines have the same order: {order}.",
                nameof(PaymentDeadlines)));
        }

        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }

    /// <summary>
    /// Gets all deadlines that are overdue as of the given date.
    /// </summary>
    public IReadOnlyList<TaxDeadline> GetOverdueDeadlines(DateTimeOffset asOf)
    {
        var overdue = new List<TaxDeadline>();

        if (_declarationDeadline?.IsOverdue(asOf) == true)
            overdue.Add(_declarationDeadline);

        overdue.AddRange(_paymentDeadlines.Where(p => p.IsOverdue(asOf)));

        return overdue.AsReadOnly();
    }

    /// <summary>
    /// Gets the next upcoming deadline as of the given date.
    /// </summary>
    public TaxDeadline? GetNextDeadline(DateTimeOffset asOf)
    {
        var allDeadlines = new List<TaxDeadline>();

        if (_declarationDeadline is not null)
            allDeadlines.Add(_declarationDeadline);

        allDeadlines.AddRange(_paymentDeadlines);

        return allDeadlines
            .Where(d => d.Enabled && d.DueDate > asOf)
            .OrderBy(d => d.DueDate)
            .FirstOrDefault();
    }
}
