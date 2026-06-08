---
name: feature
description: "Implement a new full-stack feature end-to-end in this codebase. Use when: adding a new entity, adding a new module, building a new screen, implementing a new backend capability, wiring up a new API endpoint, adding a new frontend page. Covers all layers: Domain model → EF config & migration → Commands/Queries/DTOs/Mapper → API controllers → Frontend route → UI component."
argument-hint: "<feature description>  — describe the feature to implement (e.g. 'Budgets entity with CRUD')"
---

# Feature Implementation Workflow

End-to-end guide for adding a new feature to the Finanzas full-stack application.

**Feature to implement:** $ARGUMENTS

Read the reference file(s) relevant to the scope of the feature using the Read tool, then follow their steps in order.

## Scope → Reference File

| Scope | Reference |
|-------|-----------|
| Backend API (.NET) | `.claude/commands/feature/backend.md` |
| FinanceApp frontend (React Router / SSR) | `.claude/commands/feature/finance-app.md` |
| FinanceFunds frontend (Vite / SPA) | `.claude/commands/feature/funds-app.md` |

A full-stack feature typically requires all three. Read each file as you reach that layer.

## Architecture at a Glance

```
FinanceBackEnd/src/
  Finance.Domain          ← Entity models
  Finance.Persistence     ← EF Core config, DbContext, migrations
  Finance.Application     ← Commands, Queries, DTOs, Mapping, Services
  Finance.Api             ← REST controllers (Command / Query split)

FinanceFrontEnd/
  FinanceApp/             ← React Router SSR app (Next-style loader pattern)
  FinanceFunds/           ← Vite SPA (Mantine UI, Auth0, direct ApiClient calls)
```
