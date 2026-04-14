# .NET Backend Architecture

## Overview

The backend is an ASP.NET Core Web API organized as a layered solution with a custom CQRS dispatcher, per-user data isolation via EF Core global query filters, and Auth0 JWT authentication. It targets PostgreSQL and exports distributed traces to Jaeger via OpenTelemetry.

---

## Solution Structure

| Project | Responsibility |
|---|---|
| `Finance.Domain` | Entities, value objects, domain interfaces, enums, policies |
| `Finance.Application` | Commands, queries, handlers, DTOs, mappers, services |
| `Finance.Persistence` | `FinanceDbContext`, EF configurations, query filters, repositories, telemetry interceptor |
| `Finance.Authentication` | Auth0 JWT setup, authorization policies, role definitions |
| `Finance.Api` | Controllers, DI wiring, `Program.cs`, Swagger, database seeder |
| `CQRSDispatch` | Custom in-house CQRS dispatcher (no MediatR) |
| `Finance.Migrations` | EF Core migration project (separate from runtime) |
| `Finance.Application.Tests` | xUnit test project |

---

## Domain Layer

### Base Classes

All entities inherit from one of:

- `Entity` — adds `Deactivated: bool` with reflection-based `Update()`.
- `Entity<TId>` — adds `Id`.
- `AuditedEntity<TId>` — adds `CreatedAt`, `UpdatedAt`, implements `IAuditedEntity`.
- `CreditCardEntity`, `CreditCardStatementEntity` — specialized bases for credit-card sub-graph entities.

### Value Object

`Money` is a `struct` defined in `Finance.Domain/SpecialTypes/Money.cs`. It wraps `decimal`, provides arithmetic operators and implicit conversions, and is stored as a plain `decimal` column via `MoneyValueConverter`.

### Entities by Module

| Module | Key Entities |
|---|---|
| Auth | `User`, `Role`, `ResourcePermissions<TResource,TId>` (abstract + 10 concrete subclasses) |
| Identities | `Identity`, `IdentityProvider` |
| Banks | `Bank` |
| Currencies | `Currency`, `CurrencySymbol`, `CurrencyExchangeRate`, `CurrencyConversion` |
| Movements | `Movement` |
| Funds | `Fund` |
| Incomes | `Income` |
| Debits | `Debit`, `DebitOrigin` |
| CreditCards | `CreditCard`, `CreditCardIssuer`, `CreditCardStatement`, `CreditCardStatementTransaction`, `CreditCardTransaction`, `CreditCardPayment`, `CreditCardStatementAdjustment` |
| IOLInvestments | `IOLInvestment`, `IOLInvestmentAsset`, `IOLInvestmentAssetType` |
| Subscriptions | `Subscription` |
| AppModules | `AppModule`, `AppModuleType` |
| Frequencies | `Frequency` (enum entity) |

### Ownership / Permissions Model

`ResourcePermissions<TResource, TId>` is an `AuditedEntity<Guid>` with `ResourceId`, `UserId`, `PermissionLevels` (`PermissionLevelEnum[]`). Every owned entity type has a corresponding concrete permissions class (e.g. `MovementPermissions`, `FundPermissions`). These records are the foundation of the data-isolation query filters.

### Domain Policy

`ICurrencyConversionPolicy` / `CurrencyConversionPolicy` applies buy/sell exchange rates to convert `Money` amounts between currencies. Registered as scoped and injected into currency-conversion handlers.

---

## Application Layer (CQRS)

### Commands

Commands live in `Finance.Application/Commands/`. Base abstractions:

- `BaseCreateCommand<TEntity>` → `BaseCreateCommandHandler` — `Repository.AddAsync`, returns `DataResult<TEntity>`.
- `BaseUpdateCommand`, `BaseDeleteCommand`.
- `BaseActivateCommandHandler`, `BaseDeactivateCommandHandler`.
- `BaseResponselessHandler` — for fire-and-forget operations.
- `CreateResourcePermissionsCommand`, `DeleteEntityOwnerCommand`, `OwnerBaseCommand` — ownership management.

Each domain module has its own commands folder containing create, update, delete, activate/deactivate, and permissions commands.

### Queries

Queries live in `Finance.Application/Queries/`. Base abstraction: `GetAllQuery<TEntity>` with optional `IncludeDeactivated`. Handlers extend `BaseCollectionQueryHandler` returning `DataResult<List<TEntity>>`. Summary queries (`GetCurrentFundsQuery`, `GetTotalExpensesQuery`, `GetCurrentIncomesQuery`, `GetCurrentInvestmentsQuery`, `GetGeneralSummaryQuery`) are in `Queries/Summary/`.

### DTOs and Mapping

- `IMappingService` / `MappingService` — central mapping coordinator.
- `IDtoMapper<TSource, TDest>` — per-entity mapper interface.
- Registered at startup via `services.AddMappers()`.

### Service Layer

`ICRUDService<TEntity,TId,TPermissions,TCreate,TUpdate,TDelete>` defines Create / Update / Delete / Activate / Deactivate / SetOwner / DeleteOwner. Each entity has a concrete service (e.g. `MovementService`, `FundService`) that orchestrates dispatcher calls and coordinates multi-step flows (e.g. create then auto-assign ownership).

### Dispatch Context

`FinanceDispatchContext : DispatchContext` carries the fully loaded `User` entity. `FinanceDispatchContextBuilder` resolves the current user from the JWT `NameIdentifier` claim via `FinanceDbContext` and populates the context before each command or query is dispatched.

---

## CQRSDispatch Library

A fully custom dispatcher with no dependency on MediatR.

### Key Interfaces

| Interface | Purpose |
|---|---|
| `ICommand` / `ICommand<TResult>` | Command marker |
| `IContextAwareCommand<TContext>` / `IContextAwareCommand<TContext,TResult>` | Context-bearing command |
| `IQuery<TResult>` / `IContextAwareQuery<TContext,TResult>` | Query marker |
| `ICommandHandler<TCommand,TResult>` | Handler with `ExecuteAsync` |
| `IQueryHandler<TQuery,TResult>` | Handler with `ExecuteAsync` |
| `IDispatcher<TContext>` | `DispatchAsync`, `DispatchCommandAsync`, `DispatchQueryAsync` |

### Dispatch Flow

1. `Dispatcher<TContext>` looks up the handler type from `CommandHandlerTypeRegistry` (a `ConcurrentDictionary` built at startup via assembly scan).
2. Resolves the handler from DI.
3. Opens an OTel span on `ActivitySource("Finance.Api.Dispatcher")`.
4. Invokes `ExecuteAsync` via reflection.
5. Returns `CommandResult` (void ops) or `DataResult<T>` (data ops).

There are no pipeline behaviors — dispatch is a direct handler invocation.

**Handler registration**: `services.AddRequestHandlers([assembly])` scans the application assembly at startup, registers all handlers as scoped, and builds the `CommandHandlerTypeRegistry` singleton.

---

## Persistence Layer

### DbContext

`FinanceDbContext : DbContext`

- 50+ `DbSet<T>` properties covering all domain entities.
- Injects `IHttpContextAccessor` to expose `CurrentUserId` (the Auth0 `sub` claim).
- `OnModelCreating`: `ApplyConfigurationsFromAssembly` + `modelBuilder.AddQueryFilters(this)`.
- `SaveChangesAsync` hooks:
  - `AutoCreateOwnershipPermissions` — creates a `XxxPermissions` record for each newly-added owned entity.
  - `SetAuditableDefaults` — stamps `CreatedAt` / `UpdatedAt` on `IAuditedEntity` entities.

### Global Query Filters (Data Isolation)

`ModelBuilderExtensions.AddQueryFilters` configures `HasQueryFilter` on every owned entity type. The filter joins the entity's permissions table, navigates to the user's `Identities`, and checks `SourceId == context.CurrentUserId`.

Credit-card sub-entities (statements, transactions, payments, adjustments) are filtered transitively through the parent `CreditCard`'s permissions. These filters are always active and must be explicitly bypassed with `IgnoreQueryFilters()` when needed.

### EF Configuration

One `IEntityTypeConfiguration<T>` per entity in `Finance.Persistence/Configurations/`, all auto-discovered by `ApplyConfigurationsFromAssembly`.

### Type Converters (applied globally via `ConfigureConventions`)

| Converter | Effect |
|---|---|
| `MoneyValueConverter` | Stores `Money` as `decimal` |
| `NullableMoneyValueConverter` | Same for `Money?` |
| `DateTimeUtcConverter` | Ensures `DateTime` is always stored/read as UTC |
| `NullableDateTimeUtcConverter` | Same for `DateTime?` |

### Repository Pattern

`IRepository<TEntity, TId>` / `BaseRepository<TEntity, TId>` provides expression-tree-based `FilterBy`, `GetAllBy`, and full CRUD. Domain-specific repos (e.g. `AppModuleRepository`, `CurrencyRepository`) extend the base for module-specific queries.

### Database Configuration

- Provider: PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Command timeout: 120 s.
- Query splitting: `SplitQuery` (default).
- Lazy loading: disabled.
- Interceptor: `DbTelemetryInterceptor` emits an OTel span per DB command (`ActivitySource("Finance.Api.Db")`) and logs a warning for queries exceeding 500 ms.

---

## Authentication & Authorization

### JWT Authentication

Auth0 JWTs via `Microsoft.AspNetCore.Authentication.JwtBearer`. `AuthenticationExtensions.ConfigureAuth0Authentication` validates issuer, audience, lifetime, and signing key. The `NameClaimType` is set to `ClaimTypes.NameIdentifier`, making the Auth0 `sub` claim the user's identity name.

### Authorization Policies

| Policy | Requirement |
|---|---|
| `AuthenticatedPolicy` | Authenticated user |
| `OwnerPolicy` | User owns the resource |
| `AdminPolicy` | User has Admin role |
| `AdminOrOwnerPolicy` | Admin or owner (default on `CommandController`) |

### Identity Model

`Identity` links an internal `User` to an external OAuth provider via `SourceId` (the Auth0 `sub`) and `IdentityProviderEnum`. The `SourceId` is the value compared against in all ownership query filters.

---

## API Layer

### Startup Sequence (`Program.cs`)

1. Load `.env.local` (development only) via `DotNetEnv`.
2. `builder.Configuration.AddEnvironmentVariables()`.
3. `AddTelemetry(...)` — conditionally registers OpenTelemetry.
4. Configure URLs from `Urls__Http` / `Urls__Https` env vars.
5. `ConfigureDataBase` — `FinanceDbContext`, Newtonsoft JSON controllers, `DatabaseSeeder`.
6. `MainServices` — handlers, mappers, repos, services, dispatcher, CORS, health checks, memory cache.
7. `AddSwaggerWithAuth` — Swagger with JWT bearer support.
8. `ConfigureAuth0Authentication` — JWT + policies.
9. `app.MainConfiguration()` — middleware pipeline (routing, CORS, auth, controllers, health checks, Swagger UI).

### Controller Hierarchy

```
ControllerBase
  └── SecuredApiController          [ApiController]
        └── ApiBaseController<TId,TDto>  + MappingService + Dispatcher
              ├── CommandController<...>   [Authorize(AdminOrOwnerPolicy)]  ← generic CRUD
              │     POST   /                  create
              │     PUT    /{id}              update
              │     DELETE /{id}              delete
              │     PATCH  /{id}/activate
              │     PATCH  /{id}/deactivate
              │     POST   /{id}/owner/{uid}  (Admin only)
              │     DELETE /{id}/owner/{uid}  (Admin only)
              └── ApiBaseQueryController
                    └── (26 module-specific query controllers)
```

There are 23 command controllers and 26 query controllers, each corresponding to a domain module. Queries are dispatched directly via `Dispatcher.DispatchQueryAsync`; commands go through the entity's `ICRUDService`.

### Special Controllers

- `SessionController` — `GET /api/session/me` returns the current user from DB (requires `AuthenticatedPolicy`).
- `CatalogQueryController` — bulk static/lookup data.
- `SummaryQueryController` — aggregated financial summaries.

---

## Cross-cutting Concerns

### OpenTelemetry Tracing

Feature-flagged via `OTEL_ENABLED` / `OpenTelemetry__Enabled`.

- Service name: `Finance.Api`.
- Activity sources: `Finance.Api.Db` (DB interceptor), `Finance.Api.Dispatcher` (dispatcher).
- Instrumentation: HttpClient, EF Core, ASP.NET Core.
- Exporter: OTLP (default `http://localhost:4317`, configurable via `OTEL_EXPORTER_OTLP_ENDPOINT`), targeting Jaeger.

### JSON Serialization

Dual setup:
- **Newtonsoft.Json** for controllers — `ReferenceLoopHandling.Ignore`, `StringEnumConverter` (camelCase), `MoneyNewtonsoftJsonConverter`.
- **System.Text.Json** for OAS/Swagger compatibility — `MoneyJsonConverter` ensures `Money` serializes as a numeric value.

### Startup Database Seeder

`DatabaseSeeder : IHostedService` seeds currencies, app module types, app modules, IOL asset types, roles, and an admin user on first run.

### Other

| Concern | Implementation |
|---|---|
| CORS | Open policy (`AllowOriginsForCORSPolicy`) — any origin/header/method |
| Caching | `IMemoryCache` registered via `AddMemoryCache()` |
| Health check | `GET /health` (no auth) |
| Logging | `ILogger<T>` throughout; dispatcher logs dispatch start and success/failure |

---

## External Dependencies

| Dependency | Role |
|---|---|
| Auth0 | JWT authentication (JWKS validation) + Management API for user validation (`Auth0.ManagementApi`) |
| PostgreSQL | Primary database via Npgsql |
| OpenTelemetry | Distributed tracing, exported to Jaeger via OTLP |
| FluentValidation | Command validation |
| EPPlus | Excel (`.xlsx`) parsing for `UploadMovementFileCommand` |
| DotNetEnv | `.env.local` loading in development |
| Swashbuckle | OpenAPI spec + Swagger UI |
| Newtonsoft.Json | Primary HTTP serializer |
| Redis | Declared in infra compose; not directly referenced in application code |
