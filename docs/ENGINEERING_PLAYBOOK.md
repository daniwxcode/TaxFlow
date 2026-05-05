# TaxFlow — Engineering Playbook (API, Web, Vertical Slice)

Ce document définit les « skills » et pratiques indispensables pour livrer une solution API + Web de très haute qualité, performante et maintenable, en s’appuyant sur l’existant du domaine TaxFlow.

## 1) Vision d’Architecture (cible)

- **Domain First** : le domaine reste la source de vérité (déjà bien structuré dans `TaxFlow.Framework.Core`).
- **Vertical Slice Architecture** : fonctionnalités livrées par tranche (endpoint + validation + cas d’usage + persistance + tests) plutôt que couches horizontales.
- **Bounded Contexts** : séparation nette entre modules fiscaux (Assets, Calculation, Obligations, Penalties, Payments).
- **API & Web** : API REST stable, Web client consommateur (responsabilités séparées).

### Structure de référence (API)

```
/TaxFlow.Api
  /Features
    /TaxRules
      CreateTaxRule
        CreateTaxRuleEndpoint.cs
        CreateTaxRuleCommand.cs
        CreateTaxRuleHandler.cs
        CreateTaxRuleValidator.cs
        CreateTaxRuleResponse.cs
        CreateTaxRuleTests.cs
  /Infrastructure
  /Shared
```

## 2) Vertical Slice — Règles clés

- **Chaque slice est autonome** (endpoint, DTO, validation, tests, mapping).
- **Pas de dépendance transversale** aux autres features (sauf shared).
- **Handlers minces** : orchestration + appel du domaine.
- **Domain pur** : pas de dépendance HTTP/DB dans le domaine.

### Checklist d’une slice

- Contrat d’entrée/sortie (DTO)
- Validation (règles métier + syntaxe)
- Mapping vers domaine
- Cas d’usage (handler)
- Persistance (repository / gateway)
- Tests (unitaires + contrat)

## 3) API — Standards de Qualité

- **REST explicite** : ressources claires, verbs corrects.
- **Versioning** : `/v1` dès le départ.
- **Erreurs standardisées** : problème JSON (`application/problem+json`).
- **Validation** : messages localisés (réutiliser `ValidationMessages`).
- **Paginations** : `page`, `pageSize`, `totalCount`.
- **Idempotence** : `Idempotency-Key` pour POST sensibles.
- **Observabilité** : correlation id (`X-Correlation-Id`).

## 4) Web — Standards UI/UX

- **Design system léger** : typographie, couleurs, spacing, composants de base.
- **Accessibilité** : contrastes, navigation clavier, labels.
- **Performance** : lazy loading, bundle splitting, caching.
- **Sécurité** : CSP, anti‑XSS, gestion tokens.

## 5) Performance & Fiabilité

- **Hot paths** : calculs taxes et pénalités mesurés (benchmark).
- **Cache** : règles fiscales / référentiels en mémoire (TTL + invalidation).
- **Async IO** : DB et appels externes non bloquants.
- **Timeouts** : tous les appels externes.
- **Bulk** : opérations en batch pour imports.

## 6) Qualité & Tests

- **Pyramide de tests** :
  - Unitaires (domaine)
  - Intégration (DB, API)
  - Contrat (API)
  - E2E (Web)
- **Gates CI** : lint + tests + couverture.
- **Snapshots** pour payloads API critiques.

## 7) Sécurité & Conformité

- **Input validation** partout.
- **Audit trail** (déjà prévu via `IAuditable`).
- **Permissions** par feature (RBAC/ABAC).
- **Logs sans données sensibles**.

## 8) Observabilité

- **Logging structuré** (JSON, niveaux, context).
- **Traces** sur opérations fiscales longues.
- **Metrics** : temps de calcul, volume, taux d’erreur.

## 9) Definition of Done (DoD)

- Tests unitaires + intégration
- Contrats API documentés
- Logs + métriques ajoutés
- Performance validée (budget défini)
- Revue sécurité

## 9.1) Règle de commits (traçabilité)

- **Commits petits et fréquents** (scope limité) pour faciliter le suivi et les revues.
- **1 commit = 1 intention** (ex: “add migration”, “add endpoint AssetType”).
- **Messages clairs** (type + action), éviter les commits “fourre‑tout”.

## 10) Plan d’implémentation recommandé

1. **Créer le projet API** (si absent) et la structure `Features/`.
2. **Ajouter 2 slices pilotes** : ex. `CreateAssetType`, `CalculateTaxes`.
3. **Mettre en place le socle cross-cutting** : validation, errors, logging.
4. **Ajouter tests et CI gates**.
5. **Brancher un Web client** sur les slices stables.

---

Ce playbook sert de base d’exécution. Dites-moi si vous voulez que je crée l’API et un premier ensemble de slices.
