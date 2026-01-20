# TaxFlow - Référence API

## Table des Matières

1. [Module Assets](#module-assets)
2. [Module Calculation](#module-calculation)
3. [Module Obligations](#module-obligations)
4. [Module Penalties](#module-penalties)
5. [Module Payments](#module-payments)
6. [Contracts & Validation](#contracts--validation)

---

## Module Assets

### AssetType

Agrégat racine représentant un type d'actif.

```csharp
namespace Core.Domain.Tax.Assets;

public class AssetType : TemporalAuditableEntity, IAggregateRoot
{
    // Propriétés
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<AttributeDefinition> ExpectedAttributes { get; }
    public IReadOnlyCollection<TaxRule> TaxRules { get; }

    // Factory
    public static AssetType Create(string name, string? description = null);

    // Configuration des attributs
    public AssetType AddExpectedAttribute(AttributeDefinition definition);
    public bool RemoveExpectedAttribute(string key);
    public AssetType ClearExpectedAttributes();

    // Configuration des règles fiscales
    public AssetType AddTaxRule(TaxRule rule);
    public bool RemoveTaxRule(string key);
    public TaxRule? GetTaxRule(string key);
    public AssetType UpdateRuleName(string oldKey, string newKey, string? newLabel = null);
    public AssetType ClearTaxRules();

    // Validation
    public IEnumerable<string> ValidateAttributes(IReadOnlyCollection<ExtendedAttribute> attributes);
    public ValidationResult ValidateAttributesResult(IReadOnlyCollection<ExtendedAttribute> attributes);

    // Évaluation
    public decimal? EvaluateTaxRule(
        string ruleKey,
        IReadOnlyCollection<ExtendedAttribute> attributes,
        decimal? baseAmount = null);

    // Renommage
    public void Rename(string newName);
}
```

### TaxableAsset

Instance d'un actif imposable.

```csharp
namespace Core.Domain.Tax.Assets;

public class TaxableAsset : TemporalAuditableEntity
{
    // Propriétés
    public AssetType AssetType { get; }
    public IReadOnlyCollection<ExtendedAttribute> Attributes { get; }

    // Factory
    public static TaxableAsset Create(
        AssetType assetType,
        IReadOnlyCollection<ExtendedAttribute> attributes);

    // Calcul
    public IReadOnlyCollection<TaxLine> CalculateTaxLines(
        decimal? baseAmount = null,
        DateTimeOffset? forDate = null);

    public TaxCalculationResult CalculateTaxes(TaxEngineOptions? options = null);
}
```

### AttributeValidator

Validateur d'attributs.

```csharp
namespace Core.Domain.Tax.Assets;

public static class AttributeValidator
{
    public static ValidationResult Validate(
        IReadOnlyCollection<ExtendedAttribute> attributes,
        IReadOnlyCollection<AttributeDefinition> definitions);

    public static ValidationResult ValidateAttribute(
        ExtendedAttribute attribute,
        AttributeDefinition definition);
}
```

---

## Module Calculation

### TaxRule

Règle fiscale avec expression dynamique.

```csharp
namespace Core.Domain.Tax.Calculation;

public class TaxRule : TemporalAuditableEntity
{
    // Propriétés
    public string Key { get; set; }
    public string Label { get; set; }
    public string Expression { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public TaxObligationSchedule? ObligationSchedule { get; private set; }

    // Helpers
    public bool HasDeclarationDeadline { get; }
    public bool HasPaymentDeadlines { get; }

    // Configuration
    public TaxRule ConfigureObligationSchedule(TaxObligationSchedule schedule);
    public TaxObligationSchedule GetOrCreateObligationSchedule();

    // Requêtes
    public IReadOnlyList<TaxDeadline> GetOverdueDeadlines(DateTimeOffset asOf);
}
```

### TaxEngine

Moteur de calcul des taxes.

```csharp
namespace Core.Domain.Tax.Calculation;

public static class TaxEngine
{
    public static TaxCalculationResult Evaluate(
        TaxableAsset asset,
        TaxEngineOptions? options = null);

    public static TaxCalculationResult EvaluateForPeriod(
        TaxableAsset asset,
        DateTimeOffset from,
        DateTimeOffset to,
        int daysInYear = 365,
        TaxEngineOptions? options = null);
}
```

### TaxEngineOptions

Options de calcul.

```csharp
namespace Core.Domain.Tax.Calculation;

public class TaxEngineOptions
{
    public DateTimeOffset? ForDate { get; init; }
    public decimal? BaseAmount { get; init; }
    public string? Currency { get; init; }
    public int? Precision { get; init; }
    public MidpointRounding? Rounding { get; init; }
    public bool StrictValidation { get; init; } = false;
    public bool IncludeRuleResults { get; init; } = false;
}
```

### TaxCalculationResult

Résultat du calcul.

```csharp
namespace Core.Domain.Tax.Calculation;

public sealed class TaxCalculationResult
{
    public IReadOnlyList<TaxLine> Lines { get; init; }
    public decimal Total { get; }
    public decimal? RoundedTotal { get; }
    public string? Currency { get; init; }
    public IReadOnlyList<TaxRuleEvaluationResult>? RuleResults { get; init; }
    public IReadOnlyList<string> Warnings { get; init; }
    public bool HasWarnings { get; }
}
```

### TaxLine

Ligne de résultat.

```csharp
namespace Core.Domain.Tax.Calculation;

public class TaxLine : AuditableEntity
{
    public TaxLine(string key, string label, decimal amount);
    public TaxLine(string key, string label, decimal amount, 
        string? currency, int? precision, MidpointRounding? rounding);

    public string Key { get; }
    public string Label { get; }
    public decimal Amount { get; }
    public string? Currency { get; }
    public int? Precision { get; }
    public MidpointRounding? Rounding { get; }
    public decimal? RoundedAmount { get; }
}
```

### IExpressionEvaluator

Interface d'évaluation d'expressions.

```csharp
namespace Core.Domain.Tax.Calculation;

public interface IExpressionEvaluator
{
    object? Evaluate(string expression, IDictionary<string, object?> parameters);
    bool TryEvaluate(string expression, IDictionary<string, object?> parameters, 
        out object? result, out string? error);
}
```

---

## Module Obligations

### TaxObligationSchedule

Calendrier des obligations.

```csharp
namespace Core.Domain.Tax.Obligations;

public sealed class TaxObligationSchedule : AuditableEntity
{
    // Propriétés
    public DeclarationDeadline? DeclarationDeadline { get; }
    public IReadOnlyList<PaymentDeadline> PaymentDeadlines { get; }
    public int InstallmentCount { get; }
    public bool HasDeclarationDeadline { get; }
    public bool HasPaymentDeadlines { get; }

    // Factory
    public static TaxObligationSchedule Create();

    // Configuration
    public TaxObligationSchedule WithDeclarationDeadline(DeclarationDeadline deadline);
    public TaxObligationSchedule AddPaymentDeadline(PaymentDeadline deadline);
    public bool RemovePaymentDeadline(string key);
    public TaxObligationSchedule ClearDeclarationDeadline();
    public TaxObligationSchedule ClearPaymentDeadlines();

    // Validation
    public ValidationResult Validate();

    // Requêtes
    public IReadOnlyList<TaxDeadline> GetOverdueDeadlines(DateTimeOffset asOf);
    public TaxDeadline? GetNextDeadline(DateTimeOffset asOf);
}
```

### TaxDeadline (Abstract)

Classe de base pour les échéances.

```csharp
namespace Core.Domain.Tax.Obligations;

public abstract class TaxDeadline : AuditableEntity
{
    public string Key { get; init; }
    public string Label { get; init; }
    public abstract DeadlineType Type { get; }
    public DateTimeOffset DueDate { get; init; }
    public Duration GracePeriod { get; init; }
    public DateTimeOffset EffectiveDueDate { get; }
    public string? Description { get; init; }
    public bool Enabled { get; set; }

    public bool IsOverdue(DateTimeOffset asOf);
    public int GetDaysLate(DateTimeOffset asOf);
    public TimeSpan GetTimeOverdue(DateTimeOffset asOf);
}
```

### DeclarationDeadline

Échéance de déclaration.

```csharp
namespace Core.Domain.Tax.Obligations;

public sealed class DeclarationDeadline : TaxDeadline
{
    public override DeadlineType Type => DeadlineType.Declaration;
    public PenaltyDefinition? PenaltyDefinition { get; }

    public static DeclarationDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        Duration gracePeriod = default);

    public DeclarationDeadline WithPenalty(PenaltyDefinition definition);
    public bool HasPenalty(DateTimeOffset asOf);
}
```

### PaymentDeadline

Échéance de paiement.

```csharp
namespace Core.Domain.Tax.Obligations;

public sealed class PaymentDeadline : TaxDeadline
{
    public override DeadlineType Type => DeadlineType.Payment;
    public decimal Fraction { get; init; }
    public int Order { get; init; }
    public PenaltyDefinition? PenaltyDefinition { get; }

    public static PaymentDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        decimal fraction = 1.0m,
        int order = 1,
        Duration gracePeriod = default);

    public PaymentDeadline WithPenalty(PenaltyDefinition definition);
    public decimal GetAmountDue(decimal totalTaxAmount);
    public bool HasPenalty(DateTimeOffset asOf);
}
```

### Duration

Type pour les durées flexibles.

```csharp
namespace Core.Domain.Tax.Penalties;

public readonly record struct Duration
{
    public int Value { get; init; }
    public TimeUnit Unit { get; init; }

    public Duration(int value, TimeUnit unit);

    // Factory methods
    public static Duration Zero { get; }
    public static Duration Days(int value);
    public static Duration Weeks(int value);
    public static Duration Months(int value);
    public static Duration Years(int value);

    // Opérations
    public DateTimeOffset AddTo(DateTimeOffset date);
    public int ToDays();

    // Conversion implicite
    public static implicit operator Duration(int days);
}

public enum TimeUnit { Days = 1, Weeks = 2, Months = 3, Years = 4 }
```

### ObligationPenaltyCalculator

Calculateur de pénalités basé sur les obligations.

```csharp
namespace Core.Domain.Tax.Obligations;

public sealed class ObligationPenaltyCalculator
{
    public ObligationPenaltyCalculator(PenaltyPolicy? defaultPolicy = null);

    public static ObligationPenaltyCalculator Default { get; }

    public ObligationPenaltyResult Calculate(
        TaxRule rule,
        decimal taxAmount,
        DateTimeOffset asOf,
        IReadOnlyDictionary<string, decimal>? payments = null);
}

public sealed class ObligationPenaltyResult
{
    public IReadOnlyList<PenaltyAccrual> DeclarationPenalties { get; init; }
    public IReadOnlyDictionary<string, IReadOnlyList<PenaltyAccrual>> PaymentPenalties { get; init; }
    public IReadOnlyList<PenaltyAccrual> AllPenalties { get; }
    public decimal TotalAmount { get; }
    public decimal TotalDeclarationPenalty { get; }
    public decimal TotalPaymentPenalty { get; }
}
```

---

## Module Penalties

### PenaltyPolicy

Politique de pénalités.

```csharp
namespace Core.Domain.Tax.Penalties;

public sealed class PenaltyPolicy
{
    public int DaysInYear { get; init; } = 365;
    public decimal MinimumLineAmount { get; init; } = 0m;
    public IReadOnlyCollection<PenaltyDefinition> Definitions { get; }

    public void AddOrUpdateDefinition(PenaltyDefinition definition);
    public PenaltyDefinition? GetDefinition(PenaltyType type);
    public IReadOnlyCollection<PenaltyDefinition> GetDefinitionsForEvent(PenaltyTriggerEvent triggerEvent);
    public void Validate();
}
```

### PenaltyDefinition

Définition d'une pénalité.

```csharp
namespace Core.Domain.Tax.Penalties;

public sealed class PenaltyDefinition
{
    public PenaltyType Type { get; init; }
    public PenaltyTriggerEvent TriggerEvent { get; init; } = PenaltyTriggerEvent.Any;
    public decimal FixedAmount { get; init; } = 0m;
    public Duration GracePeriod { get; init; } = Duration.Zero;
    public Duration Period { get; init; } = Duration.Days(30);
    public decimal AnnualRate { get; init; } = 0m;
    public decimal PeriodRate { get; init; } = 0m;
    public decimal PeriodRateIncrement { get; init; } = 0m;
    public decimal? Cap { get; init; }
    public decimal? Minimum { get; init; }
    public bool Capitalize { get; init; } = false;

    public void Validate();
    public DateTimeOffset GetEffectiveDueDate(DateTimeOffset dueDate);
    public DateTimeOffset GetPeriodStartDate(DateTimeOffset effectiveDueDate, int periodIndex);
    public DateTimeOffset GetPeriodEndDate(DateTimeOffset effectiveDueDate, int periodIndex);
}
```

### PenaltyCalculator

Calculateur de pénalités (avec PaymentSchedule).

```csharp
namespace Core.Domain.Tax.Penalties;

public static class PenaltyCalculator
{
    public static PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        DateTimeOffset? assietteDueDate = null,
        PenaltyTriggerEvent triggerEvent = PenaltyTriggerEvent.Any);
}
```

### PenaltyAccrual

Ligne de pénalité calculée.

```csharp
namespace Core.Domain.Tax.Penalties;

public sealed record PenaltyAccrual(
    PenaltyType Type,
    PenaltyLineType LineType,
    Guid DeclarationId,
    Guid? LiquidationId,
    Guid? InstallmentId,
    decimal BaseAmount,
    decimal Amount,
    decimal Rate,
    int DaysLate,
    int PeriodIndex,
    int PeriodDays,
    DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd,
    DateTimeOffset CalculatedAt);
```

### Énumérations

```csharp
namespace Core.Domain.Tax.Penalties;

public enum PenaltyType { Assiette = 1, Recouvrement = 2 }

public enum PenaltyLineType
{
    AssietteFixed = 1,
    AssietteRate = 2,
    RecouvrementFixed = 3,
    RecouvrementRate = 4
}

[Flags]
public enum PenaltyTriggerEvent
{
    None = 0,
    LateDeclaration = 1,
    LatePayment = 2,
    Any = LateDeclaration | LatePayment
}
```

---

## Module Payments

### PaymentSchedule

Échéancier de paiement.

```csharp
namespace Core.Domain.Tax.Payments;

public class PaymentSchedule
{
    public Guid DeclarationId { get; }
    public Guid? LiquidationId { get; }
    public IReadOnlyList<Installment> Installments { get; }
    public IReadOnlyList<Payment> Payments { get; }
    public decimal TotalDue { get; }
    public decimal TotalPaid { get; }
    public decimal TotalOutstanding { get; }

    public PaymentSchedule(Guid declarationId, Guid? liquidationId, 
        IEnumerable<Installment> installments);

    public void ApplyPayment(Payment payment, 
        AllocationStrategy strategy = AllocationStrategy.Fifo);
}
```

### Installment

Échéance de paiement.

```csharp
namespace Core.Domain.Tax.Payments;

public class Installment
{
    public Guid Id { get; }
    public decimal Amount { get; }
    public DateTimeOffset DueDate { get; }
    public DateTimeOffset EffectiveDueDate { get; }
    public IReadOnlyList<PaymentAllocation> Allocations { get; }

    public Installment(Guid id, decimal amount, DateTimeOffset dueDate);

    public decimal GetOutstanding(DateTimeOffset asOf);
}
```

### Payment

Paiement reçu.

```csharp
namespace Core.Domain.Tax.Payments;

public class Payment
{
    public Guid Id { get; }
    public decimal Amount { get; }
    public DateTimeOffset Date { get; }

    public Payment(Guid id, decimal amount, DateTimeOffset date);
}
```

---

## Contracts & Validation

### ValidationResult

Résultat de validation structuré.

```csharp
namespace Core.Domain.Contracts.Validation;

public sealed class ValidationResult
{
    public bool IsValid { get; }
    public bool HasErrors { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
    public string ErrorMessage { get; }

    public static ValidationResult Success();
    public static ValidationResult Failure(ValidationError error);
    public static ValidationResult Failure(IEnumerable<ValidationError> errors);
    public static ValidationResult Combine(params ValidationResult[] results);
}
```

### ValidationError

Erreur de validation.

```csharp
namespace Core.Domain.Contracts.Validation;

public sealed record ValidationError(
    string Code,
    string Message,
    string? PropertyName = null);
```

### ValidationErrorCodes

Codes d'erreur standards.

```csharp
namespace Core.Domain.Contracts.Validation;

public static class ValidationErrorCodes
{
    // Attributs
    public const string DuplicateAttribute = "DUPLICATE_ATTRIBUTE";
    public const string MissingRequiredAttribute = "MISSING_REQUIRED_ATTRIBUTE";
    public const string InvalidDataType = "INVALID_DATA_TYPE";
    public const string InvalidValue = "INVALID_VALUE";
    public const string InvalidEnumValue = "INVALID_ENUM_VALUE";
    public const string MissingEnumDefinition = "MISSING_ENUM_DEFINITION";
    public const string InvalidRegexPattern = "INVALID_REGEX_PATTERN";
    public const string RegexMismatch = "REGEX_MISMATCH";

    // Règles
    public const string RuleNotFound = "RULE_NOT_FOUND";
    public const string RuleDisabled = "RULE_DISABLED";
    public const string RuleEvaluationFailed = "RULE_EVALUATION_FAILED";
    public const string MissingParameters = "MISSING_PARAMETERS";
    public const string EmptyRuleKey = "EMPTY_RULE_KEY";
}
```

---

*Documentation générée automatiquement - TaxFlow Framework v1.0*
