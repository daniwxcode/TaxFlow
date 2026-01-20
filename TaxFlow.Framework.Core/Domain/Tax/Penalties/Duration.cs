namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Represents a unit of time for periods and grace durations.
/// </summary>
public enum TimeUnit
{
    /// <summary>
    /// Duration in days.
    /// </summary>
    Days = 1,

    /// <summary>
    /// Duration in weeks (7 days).
    /// </summary>
    Weeks = 2,

    /// <summary>
    /// Duration in months.
    /// </summary>
    Months = 3,

    /// <summary>
    /// Duration in years.
    /// </summary>
    Years = 4
}

/// <summary>
/// Represents a duration with a value and unit.
/// </summary>
public readonly record struct Duration
{
    /// <summary>
    /// The numeric value of the duration.
    /// </summary>
    public int Value { get; init; }

    /// <summary>
    /// The unit of time.
    /// </summary>
    public TimeUnit Unit { get; init; }

    /// <summary>
    /// Creates a duration with the specified value and unit.
    /// </summary>
    public Duration(int value, TimeUnit unit)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Duration value must be non-negative.");

        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Zero duration.
    /// </summary>
    public static Duration Zero => new(0, TimeUnit.Days);

    /// <summary>
    /// Creates a duration in days.
    /// </summary>
    public static Duration Days(int value) => new(value, TimeUnit.Days);

    /// <summary>
    /// Creates a duration in weeks.
    /// </summary>
    public static Duration Weeks(int value) => new(value, TimeUnit.Weeks);

    /// <summary>
    /// Creates a duration in months.
    /// </summary>
    public static Duration Months(int value) => new(value, TimeUnit.Months);

    /// <summary>
    /// Creates a duration in years.
    /// </summary>
    public static Duration Years(int value) => new(value, TimeUnit.Years);

    /// <summary>
    /// Adds this duration to a date.
    /// </summary>
    public DateTimeOffset AddTo(DateTimeOffset date) => Unit switch
    {
        TimeUnit.Days => date.AddDays(Value),
        TimeUnit.Weeks => date.AddDays(Value * 7),
        TimeUnit.Months => date.AddMonths(Value),
        TimeUnit.Years => date.AddYears(Value),
        _ => date.AddDays(Value)
    };

    /// <summary>
    /// Converts this duration to an approximate number of days.
    /// Note: For months and years, this is an approximation.
    /// </summary>
    public int ToDays() => Unit switch
    {
        TimeUnit.Days => Value,
        TimeUnit.Weeks => Value * 7,
        TimeUnit.Months => Value * 30, // Approximation
        TimeUnit.Years => Value * 365, // Approximation
        _ => Value
    };

    public override string ToString() => Unit switch
    {
        TimeUnit.Days => $"{Value} jour(s)",
        TimeUnit.Weeks => $"{Value} semaine(s)",
        TimeUnit.Months => $"{Value} mois",
        TimeUnit.Years => $"{Value} an(s)",
        _ => $"{Value} {Unit}"
    };

    /// <summary>
    /// Implicit conversion from int (assumes days for backward compatibility).
    /// </summary>
    public static implicit operator Duration(int days) => Days(days);
}
