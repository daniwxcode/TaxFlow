# TaxFlow

TaxFlow est une bibliothèque/plateforme modulaire pour définir des types d'actifs, déclarer des règles fiscales dynamiques et calculer automatiquement des lignes de taxe pour des actifs munis d'attributs étendus et temporels.

## Objectif

Fournir un noyau simple, extensible et testable permettant de :
- Définir des `AssetType` (types d'actifs) avec des attributs attendus (obligatoires, types, énumérations, regex).
- Attacher des `TaxRule` dynamiques (expressions NCalc) à ces `AssetType`.
- Calculer des lignes de taxe (`TaxLine`) pour un actif (`TaxableAsset`) à partir de ses attributs et des règles actives, en tenant compte de la validité temporelle des attributs.
- Faciliter l’extension vers la persistance, une API ou une interface utilisateur.

## Concepts clés
- `AssetType` : définition d’un type d’actif (nom, description), liste d’attributs attendus et règles fiscales (`TaxRule`).
- `AttributeDefinition` : décrit une clé d’attribut, son type (`Number`, `String`, `Enum`, `Date`, ...), si requis, pattern regex ou définition d’enum.
- `ExtendedAttribute` : valeur d’un attribut attachée à un actif ; contient métadonnées temporelles (`ValidFrom`, `ValidTo`) et type.
- `TaxRule` : expression dynamique qui calcule une valeur (NCalc) ; possède `Key`, `Label`, `Expression` et indicateur `Enabled`.
- `TaxableAsset` : instance d’un actif, référence à un `AssetType` et collection d’`ExtendedAttribute`. Méthode principale : `CalculateTaxLines`.
- `TaxLine` : résultat par règle (`Key`, `Label`, `Amount`).

## Fonctionnalités principales
- Validation des attributs fournis par rapport à la définition du `AssetType`.
- Expressions dynamiques pour les règles fiscales (NCalc). Les variables disponibles sont les clés d’attributs et une variable optionnelle `amount`.
- Support de la validité temporelle des attributs : calculs sur la valeur des attributs valides à une date donnée.
- Résultat détaillé : une ligne par règle permettant audit et affichage.

## Exemple d’utilisation

```csharp
// Créer un AssetType, ajouter des règles et des attributs attendus
var at = AssetType.Create("Real Estate");
at.AddExpectedAttribute(AttributeDefinition.Create("ResidualValue", "Valeur Venale", AttributeDataType.Number, true));

at.AddTaxRule(new TaxRule {
    Key = "TFB",
    Label = "Taxe foncière bâtie",
    Expression = "[ResidualValue] * 0.0075" // 0.75%
});

// Créer un actif taxable avec ses attributs
var attributes = new Collection<ExtendedAttribute> {
    ExtendedAttribute.Create("ResidualValue", "1000000", AttributeDataType.Number, true)
};
var asset = TaxableAsset.Create(at, attributes);

// Calculer les lignes de taxe
var lines = asset.CalculateTaxLines(); // retourne IReadOnlyCollection<TaxLine>
```

## Bonnes pratiques
- Tester et valider les expressions NCalc avant production.
- Utiliser des clés d’attributs stables et simples.
- Gérer la désactivation des règles via `Enabled` plutôt que suppression immédiate.

## Extensibilité
- Persistance : ajouter un `DbContext` EF Core pour stocker les entités.
- API/UI : exposer la gestion des `AssetType`, `TaxRule` et `TaxableAsset` et l'exécution des calculs.
- Tests : créer des tests unitaires pour la validation des attributs, l'évaluation des règles et les scénarios temporels.

## Sécurité
- Ne jamais évaluer des expressions provenant de sources non fiables sans contrôles et sandboxing.
- Journaliser les erreurs d’évaluation et valider les entrées utilisateur.

## Démarrage rapide (dev)
1. Cloner le dépôt.
2. Restaurer les packages : `dotnet restore`.
3. Compiler : `dotnet build`.
4. Ajouter/Exécuter des tests : `dotnet test` (ajouter un projet de tests si nécessaire).

## Licence
Préciser la licence souhaitée (ex : MIT).