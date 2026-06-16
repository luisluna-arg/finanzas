# Finanzas — Agent Reference

## What This Project Is

Finanzas is a personal finance web application. It tracks movements, funds, incomes, debits, credit cards, investments, and subscriptions per authenticated user. All financial data is strictly isolated per user via EF Core global query filters tied to Auth0 identity.

---

## Repository Layout

```
FinanceBackEnd/            ← ASP.NET Core Web API (.NET)
FinanceFrontEnd/
  FinanceApp/              ← React Router v7 SSR app (main UI)
  FinanceFunds/            ← Vite SPA (funds + exchange rates)
.infra/local/              ← Docker Compose stacks (infra, FinanceApp, FinanceFunds)
.bin/powershell/           ← Start scripts
.github/docs/              ← Developer reference docs (CQRS, query filters, unit testing, local dev)
docs/                      ← Architecture docs, security audits, performance notes
.claude/commands/          ← Claude Code slash commands
.github/skills/            ← GitHub Copilot chat skills
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | C# / ASP.NET Core Web API, EF Core, PostgreSQL, Redis, Auth0 JWT, OpenTelemetry |
| FinanceApp | React Router v7 (SSR), Tailwind CSS, shadcn/ui, Axios, Auth0 (server-side), Redis sessions |
| FinanceFunds | React 19 SPA, Vite, Mantine v8, Auth0 SDK (client-side), native fetch |

---

## Architecture

Detailed docs live in `docs/architecture/`:

- [`docs/architecture/backend-dotnet.md`](docs/architecture/backend-dotnet.md) — solution structure, CQRS, EF Core, auth, API layer
- [`docs/architecture/frontend-financeapp.md`](docs/architecture/frontend-financeapp.md) — SSR routing, session, proxy, UI conventions
- [`docs/architecture/frontend-financefunds.md`](docs/architecture/frontend-financefunds.md) — SPA routing, Auth0 SDK, API client, services

Developer pattern references:

- [`.github/docs/cqrs-pattern.md`](.github/docs/cqrs-pattern.md) — command/query result types, dispatcher, base classes
- [`.github/docs/finance-query-filters.md`](.github/docs/finance-query-filters.md) — EF Core global query filters and ownership model
- [`.github/docs/unit-testing.md`](.github/docs/unit-testing.md) — test conventions, base classes, seeding
- [`.github/docs/local-dev-environment.md`](.github/docs/local-dev-environment.md) — ports, Docker stacks, env files, known gotchas

---

## Build Commands

### Backend

```powershell
dotnet build FinanceBackEnd/src/Finance.Api
dotnet test FinanceBackEnd/tests/Finance.Application.Tests
dotnet ef migrations add <Name> --project FinanceBackEnd/src/Finance.Migrations --startup-project FinanceBackEnd/src/Finance.Api
dotnet ef database update --project FinanceBackEnd/src/Finance.Migrations --startup-project FinanceBackEnd/src/Finance.Api
```

### FinanceApp (React Router SSR) — port 5100

```powershell
cd FinanceFrontEnd/FinanceApp
npm run dev
npm run build
npm run typecheck
npm run lint
```

### FinanceFunds (Vite SPA) — port 5200

```powershell
cd FinanceFrontEnd/FinanceFunds
npm run dev
npm run build
npm run lint
```

---

## Coding Conventions

- Write self-documenting code with clear names. Prefer refactoring unclear code into well-named methods over adding explanatory comments.
- Only add comments for: complex algorithms, non-obvious business rules, public API XML docs, workarounds. Never for obvious code.
- Comments explain *why*, not *what* — the code shows what.
- Never use `#pragma warning disable` — fix the underlying issue instead.
- Always reuse existing UI components instead of falling back to raw HTML elements. Check imports already in the file, then look in these two locations before writing any markup:
  - `app/components/ui/shadcn/` — `Button`, `Input`, `Label`, `Separator`, `Textarea`, `Table/*`, `Carousel/*`, `Pagination`
  - `app/components/ui/utils/` — `Checkbox`, `Picker` (select/dropdown), `Uploader` (file upload with toast), `InputControl`, `Modal/*`, `ActionButton`, `BankCurrencySelector`, `Table` (simple data+columns), `FetchTable` (data or URL, empty state, totals, collapsible), `PaginatedTable` (paginated + CRUD)

### Backend: Folder Structure

Folders are organised **category first, then domain**: `Commands/CreditCards/`, `Specifications/CreditCards/`, `Queries/CreditCards/`, etc.

Auth specifications (role/identity checks) live in `Auth/`. Business-rule specifications live in `Specifications/<Domain>/` and are registered via `AddSpecifications()` in `ServiceExtensions`.

### Backend: Command Error Handling

The desired pattern is the **envelope approach**: command handlers return `DataResult<TEntity>` (or `DataResult.Failure("reason")` for expected failures), and controllers unwrap the envelope and map to the appropriate HTTP status code.

`CommandController` follows this correctly — it checks `result.IsSuccess` and returns `BadRequest(result.ErrorMessage)` on failure.

`ApiBaseCUDCommandController` is a **known inconsistency**: it calls `DispatchCommandAsync` and always returns `Ok()`, ignoring the result. Until that controller is fixed, handlers behind it must `throw` to abort a request (use `UnauthorizedAccessException` for permission failures, `InvalidOperationException` for business rule violations). Do not silently swallow failures by adjusting the record and proceeding.

---

## Code Changes Protocol

1. **Review before implementing** — search and read relevant files, understand existing patterns, identify impacts, ask if requirements are ambiguous.
2. **Propose before doing** — present your findings and proposed approach before making changes; wait for confirmation.
3. **Exception** — implement directly only when explicitly told to ("just do it"), when the change is trivial (typo, formatting), or when you have already reviewed and proposed in the same conversation.
4. **Document new patterns** — when exploration reveals a pattern not yet in this file (architectural convention, error handling approach, naming rule, etc.), confirm with the user whether it is the desired pattern before adopting it, then add it here.

---

## Dev Environment

- Never run `git commit`, `git push`, or `git rebase` unless explicitly instructed.
- See [`.github/docs/local-dev-environment.md`](.github/docs/local-dev-environment.md) for Docker stacks, env file locations, and the localhost/IPv6 gotcha.
