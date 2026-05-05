# Base de Données — PostgreSQL (recommandé)

Ce dossier contient le schéma initial pour PostgreSQL et les conventions associées.

## Pourquoi PostgreSQL

- Modèle relationnel fiable et transactionnel pour le domaine fiscal.
- Support JSONB (utile pour extensions futures).
- Indexation et performances éprouvées.

## Schéma initial

Le fichier [postgresql.schema.sql](postgresql.schema.sql) fournit :

- Tables de référentiel : `asset_types`, `enum_definitions`, `enum_items`, `attribute_definitions`
- Règles fiscales : `tax_rules`
- Obligations : `tax_obligation_schedules`, `declaration_deadlines`, `payment_deadlines`
- Références légales : `legal_references` + tables de jointure

## Notes de mapping

- Les champs d’audit sont présents sur toutes les entités.
- Les durées (`Duration`) sont stockées en `jsonb` (ex: `grace_period`, `penalty_period`).
- Les pénalités sont stockées dans les colonnes `penalty_*` des tables d’échéances.
- Les traductions (`LocalizedString`) sont stockées en `jsonb` (ex: `localized_label`, `localized_description`).

## Persistance .NET

Le mapping EF Core se trouve dans :

- [TaxFlow.Infrastructure/Persistence/TaxFlowDbContext.cs](../../TaxFlow.Infrastructure/Persistence/TaxFlowDbContext.cs)
- [TaxFlow.Infrastructure/Persistence/Configurations](../../TaxFlow.Infrastructure/Persistence/Configurations)
