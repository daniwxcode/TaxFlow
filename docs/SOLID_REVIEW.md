# Revue SOLID - TaxFlow Framework Core

## Résumé Exécutif

Ce document identifie et propose des corrections pour les violations des principes SOLID détectées dans l'architecture du domaine après réorganisation en `Entities` et `Services`.

### Points Positifs ?
- ? Séparation claire Entities/Services
- ? Utilisation d'abstractions (IExpressionEvaluator, IPenaltyRule)
- ? Immutabilité des entités
- ? Validation structurée

### Violations Identifiées ??

---

## 1. Single Responsibility Principle (SRP)

### ? Violation 1.1: TaxRuleEvaluator - Trop de responsabilités

**Fichier:** `Domain/Tax/Calculation/Services/TaxRuleEvaluator.cs`

**Problème:**
- Construit les paramètres d'évaluation (SRP #1)
- Gère les énumérations (SRP #2)
- Convertit les types de données (SRP #3)
- Évalue les règles (SRP #4)

```csharp
public TaxRuleEvaluationResult Evaluate(...)
{
    // SRP1: Construction des paramètres
    var parameters = BuildParameters(...);
    
    // SRP2: Évaluation
    var evalResult = _expressionEvaluator.Evaluate(...);
    
    // SRP3: Gestion des erreurs/warnings
    var warnings = evalResult.MissingParameters.Count > 0 ? ... : [];
}
```

**Impact:** Couplage fort, difficulté de tester individuellement chaque aspect.

**Solution:** Extraire en services spécialisés:
1. `TaxRuleParameterBuilder` - Construire les paramètres
2. `EnumParameterHandler` - Gérer les énumérations
3. `ParameterTypeConverter` - Convertir les types

---

### ? Violation 1.2: TaxEngine - Stateless vs Complexity

**Fichier:** `Domain/Tax/Calculation/Services/TaxEngine.cs`

**Problème:** 18+ méthodes privées, complexité élevée, difficile à évoluer.

```csharp
public static TaxCalculationResult Evaluate(TaxableAsset asset, ...)
{
    // ~50 lignes de logique directement
    // Mélange: validation, extraction, évaluation, construction
}
```

**Solution:** Créer un `TaxCalculationOrchestrator` non-statique avec dépendances injectées.

---

### ? Violation 1.3: PenaltyCalculator - Trop de responsabilités

**Fichier:** `Domain/Tax/Penalties/Services/PenaltyCalculator.cs`

**Problème:**
- Gère les règles (hardcodé: `AssiettePenaltyRule`, `RecouvrementPenaltyRule`)
- Orchestre l'évaluation
- Agrège les résultats

**Solution:** Utiliser le pattern `Strategy` + `Factory` pour les règles.

---

## 2. Open/Closed Principle (OCP)

### ? Violation 2.1: PenaltyCalculator - Fermé à l'extension

**Fichier:** `Domain/Tax/Penalties/Services/PenaltyCalculator.cs`

```csharp
private static readonly IReadOnlyList<IPenaltyRule> Rules = new IPenaltyRule[]
{
    new AssiettePenaltyRule(),
    new RecouvrementPenaltyRule()
    // Pour ajouter une nouvelle règle: modification du code existant ?
};
```

**Problème:** Pour ajouter une `ThirdPenaltyRule`, il faut modifier `PenaltyCalculator`.

**Solution:** Implémenter `IPenaltyRuleRegistry` injectable:
```csharp
public interface IPenaltyRuleRegistry
{
    IEnumerable<IPenaltyRule> GetRules();
    void Register(IPenaltyRule rule);
}
```

---

### ? Violation 2.2: TaxRuleEvaluator - Ajout d'énumérations

**Problème:** Modifier `TryAddEnumParameters` pour supporter nouvelles types d'énums.

**Solution:** Stratégie `IEnumParameterHandler` spécialisée par type d'énumération.

---

## 3. Liskov Substitution Principle (LSP)

### ? Violation 3.1: DeclarationDeadline & PaymentDeadline

**Fichier:** `Domain/Tax/Obligations/Entities/TaxDeadline.cs`

**Problème:** Les sous-classes modifient le comportement sans respecter le contrat de base.

```csharp
public abstract class TaxDeadline
{
    public PaymentType PaymentType { get; } // DeclarationDeadline doit ignorer
    public decimal Fraction { get; } // PaymentDeadline uniquement
}
```

**Solution:**
- Supprimer les propriétés inutiles des sous-classes
- Utiliser la composition plutôt que l'héritage
- Créer des interfaces spécialisées: `IDeclarationDeadline`, `IPaymentDeadline`

---

## 4. Interface Segregation Principle (ISP)

### ? Violation 4.1: IExpressionEvaluator - Trop générique

**Fichier:** `Domain/Tax/Calculation/Services/IExpressionEvaluator.cs`

```csharp
public interface IExpressionEvaluator
{
    ExpressionEvaluationResult Evaluate(string expression, IDictionary<string, object?> parameters);
}
```

**Problème:** Force les implémentations à gérer tous types de paramètres.

**Solution:** Créer des interfaces plus spécialisées:
```csharp
public interface ITaxRuleExpressionEvaluator
{
    ExpressionEvaluationResult EvaluateTaxRule(
        string expression, 
        IReadOnlyDictionary<string, decimal> numericParams,
        IReadOnlyDictionary<string, string> stringParams);
}
```

---

### ? Violation 4.2: IPenaltyRule - Interface trop chargée

```csharp
public interface IPenaltyRule
{
    IEnumerable<PenaltyAccrual> Evaluate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        DateTimeOffset? assietteDueDate,
        PenaltyTriggerEvent triggerEvent);
}
```

**Problème:** Tous les paramètres ne sont pas utiles pour chaque implémentation.

**Solution:** Créer `IAssiettePenaltyRule` et `IRecouvrementPenaltyRule` spécialisées.

---

## 5. Dependency Inversion Principle (DIP)

### ? Violation 5.1: TaxEngine - Dépendance statique

**Fichier:** `Domain/Tax/Calculation/Services/TaxEngine.cs`

**Problème:** Stateless avec méthodes statiques rend l'injection impossible.

```csharp
public static class TaxEngine
{
    // Impossible d'injecter les dépendances
    // Impossible de mocker pour les tests
}
```

**Solution:** Convertir en classe non-statique avec interface:
```csharp
public interface ITaxCalculationEngine
{
    TaxCalculationResult Evaluate(TaxableAsset asset, TaxEngineOptions? options = null);
    TaxCalculationResult EvaluateForPeriod(...);
}

public class TaxCalculationEngine : ITaxCalculationEngine
{
    private readonly ITaxRuleEvaluator _ruleEvaluator;
    private readonly IAttributeValidator _attributeValidator;
    
    public TaxCalculationEngine(ITaxRuleEvaluator ruleEvaluator, IAttributeValidator attributeValidator)
    {
        _ruleEvaluator = ruleEvaluator;
        _attributeValidator = attributeValidator;
    }
    
    public TaxCalculationResult Evaluate(TaxableAsset asset, TaxEngineOptions? options = null)
    {
        // Utiliser les dépendances injectées
    }
}
```

---

### ? Violation 5.2: PenaltyCalculator - Dépendances hardcodées

**Fichier:** `Domain/Tax/Penalties/Services/PenaltyCalculator.cs`

```csharp
private static readonly IReadOnlyList<IPenaltyRule> Rules = new IPenaltyRule[]
{
    new AssiettePenaltyRule(),  // Création directe ?
    new RecouvrementPenaltyRule()
};
```

**Solution:** Accepter une factory injectable:
```csharp
public interface IPenaltyCalculator
{
    PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        DateTimeOffset? assietteDueDate = null);
}

public class PenaltyCalculator : IPenaltyCalculator
{
    private readonly IPenaltyRuleRegistry _ruleRegistry;
    
    public PenaltyCalculator(IPenaltyRuleRegistry ruleRegistry)
    {
        _ruleRegistry = ruleRegistry;
    }
}
```

---

### ? Violation 5.3: TaxRuleEvaluator - Dépendance NCalc hardcodée

```csharp
public TaxRuleEvaluator(IExpressionEvaluator? expressionEvaluator = null)
{
    _expressionEvaluator = expressionEvaluator ?? NCalcExpressionEvaluator.Instance;
    // Fallback hardcodé ?
}
```

**Solution:** Rendre NCalcExpressionEvaluator optionnel en configuration DI uniquement.

---

## 6. Autres Problèmes

### ? Violation 6.1: Namespaces non cohérents

**Problème:** Les fichiers ont déménagé mais gardent l'ancien namespace.

```csharp
// Fichier: Domain/Tax/Calculation/Services/TaxEngine.cs
// Namespace: Core.Domain.Tax.Calculation
// ? Devrait être: Core.Domain.Tax.Calculation.Services
```

**Impact:** Confusion, violate le principle "namespace = physical location".

---

### ? Violation 6.2: Pas d'interfaces publiques

**Problème:** Services créés comme classes mais pas d'interfaces associées.

**Solution:** Créer une interface pour chaque service public.

---

## Plan de Correction

### Phase 1: Interfaces et Abstractions (Priorité: HIGH)
- [ ] Créer `ITaxCalculationEngine` pour remplacer `TaxEngine` statique
- [ ] Créer `IPenaltyCalculator` pour remplacer `PenaltyCalculator` statique
- [ ] Créer `ITaxRuleEvaluator` pour `TaxRuleEvaluator`
- [ ] Créer `IPenaltyRuleRegistry` pour gérer les règles
- [ ] Fixer les namespaces

### Phase 2: Extraction de Services (Priorité: HIGH)
- [ ] Extraire `TaxRuleParameterBuilder` de `TaxRuleEvaluator`
- [ ] Extraire `EnumParameterHandler` de `TaxRuleEvaluator`
- [ ] Créer `TaxCalculationOrchestrator` pour TaxEngine
- [ ] Créer `IAssiettePenaltyRule` et `IRecouvrementPenaltyRule` spécialisées

### Phase 3: Composition vs Héritage (Priorité: MEDIUM)
- [ ] Refactoriser `TaxDeadline` hierarchy
- [ ] Créer dedicated deadline types sans héritage

### Phase 4: Interface Segregation (Priorité: MEDIUM)
- [ ] Créer `ITaxRuleExpressionEvaluator` spécialisée
- [ ] Créer `IAssietteRuleEvaluator` et `IRecouvrementRuleEvaluator`

### Phase 5: Configuration et DI (Priorité: LOW)
- [ ] Créer module d'extension pour DI
- [ ] Ajouter patterns de factory

---

## Checkliste SOLID Post-Refactoring

- [ ] **SRP**: Chaque classe a une seule raison de changer
- [ ] **OCP**: Ajouter une nouvelle règle ne modifie pas le code existant
- [ ] **LSP**: Les sous-classes respectent le contrat de base
- [ ] **ISP**: Les interfaces sont spécifiques à chaque client
- [ ] **DIP**: Les dépendances sont inversées et injectables

---

## Références

- SOLID Principles by Robert C. Martin
- Clean Architecture
- Domain-Driven Design
