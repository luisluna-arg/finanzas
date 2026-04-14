# Security Audit — Finance.Persistence

**Date**: 2026-03-29
**Scope**: `Finance.Persistence/FinanceDbContext.cs`, `Finance.Persistence/Extensions/ModelBuilderExtensions.cs`, `Finance.Persistence/Configurations/**`, `Finance.Persistence/TypeConverters/**`, `Finance.Persistence/Telemetry/DbTelemetryInterceptor.cs`
**Reviewer**: GitHub Copilot

---

## Findings

### 1. `CurrentUserId` Falls Back to Literal String `"IdentityNotFound"` When No HTTP Context Exists

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs](FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs#L67) |
| **Lines** | [L67](FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs#L67) |
| **Description** | `CurrentUserId` is computed as `HttpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "IdentityNotFound"`. This value is embedded directly in every EF global query filter as the user identity anchor. If any code path resolves a `FinanceDbContext` outside an active HTTP request — for example, in a background job, the `DatabaseSeeder`, EF migrations, integration tests, or a `BuildServiceProvider()`-scoped instance (see the Authentication audit) — `CurrentUserId` is `"IdentityNotFound"`, a literal string that will never match any real `Identity.SourceId`. The result is that all ownership filters evaluate to `false`, making every filtered `DbSet` return zero rows rather than raising an error. A background service that accidentally modifies data would silently skip ownership checks. More critically, the `AutoCreateOwnershipPermissions` helpers at [L119–L158](FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs#L119) have an explicit `if (HttpContextAccessor?.HttpContext == null) return;` guard — but the query filters used in read paths do not share the same guard: they just silently return empty. |
| **Priority** | High |
| **Recommendation** | Change the fallback to throw `InvalidOperationException` instead of returning a harmless sentinel: `internal string CurrentUserId => HttpContextAccessor?.HttpContext?.User?.Identity?.Name ?? throw new InvalidOperationException("FinanceDbContext used outside an active HTTP context.")`. For background/seeder scenarios that legitimately need unfiltered access, use `.IgnoreQueryFilters()` explicitly on those specific queries, which makes the bypass intentional and code-reviewable. |

---

### 2. `DbTelemetryInterceptor` Logs Full SQL Command Text on Slow Queries — Data Leakage to Log Aggregators

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/Telemetry/DbTelemetryInterceptor.cs](FinanceBackEnd/src/Finance.Persistence/Telemetry/DbTelemetryInterceptor.cs#L91) |
| **Lines** | [L91](FinanceBackEnd/src/Finance.Persistence/Telemetry/DbTelemetryInterceptor.cs#L91), [L86](FinanceBackEnd/src/Finance.Persistence/Telemetry/DbTelemetryInterceptor.cs#L86) |
| **Description** | Two sinks emit the full SQL command text: (1) `activity.SetTag("db.statement", command.CommandText)` at L86, which sends the raw query to any configured OpenTelemetry collector (Jaeger, Grafana, etc.). (2) `logger.LogWarning("Slow SQL ... {Command}", ..., command.CommandText)` at L91, which emits the full parameterised or inline SQL to the structured log output. EF Core by default uses parameterised queries, so literal financial values typically do not appear in `CommandText`. However, the SQL text does contain table names, column names, and filter predicates — including the current `CurrentUserId` value. A slow-query log entry for a movement paginated query would emit the user's Auth0 source ID (`auth0|...`) as part of the WHERE clause. In environments where Jaeger or the application log is accessible to multiple engineers or sent externally, this leaks PII and user identity tokens. |
| **Priority** | High |
| **Recommendation** | Remove `db.statement` from the OpenTelemetry tag or gate it behind a compile-time or configuration flag that defaults to off in production. For the slow-query log, emit only the elapsed time and a sanitised query identifier (e.g. hash of the command text), not the full SQL. See [OpenTelemetry semantic conventions for database spans](https://opentelemetry.io/docs/specs/semconv/database/database-spans/) which explicitly classify `db.statement` as sensitive and recommend omitting it in production. |

---

### 3. No Decimal Precision Defined for Any `Money` Column — Default PostgreSQL `numeric` Precision

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs](FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs#L1) |
| **Lines** | [L1–L17](FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs#L1) |
| **Description** | `MoneyValueConverter` converts `Money` to `decimal` with no `ConverterMappingHints` specifying precision or scale. No `HasPrecision()` call appears anywhere in the entity configurations for any financial column (`Movement.Amount`, `Fund.Amount`, `Income.Amount`, `CreditCardTransaction.Amount`, `CreditCardPayment.Amount`, `CreditCardStatementAdjustment.Amount`, `CurrencyExchangeRate.BuyRate`/`SellRate`, all `IOLInvestment` money fields). PostgreSQL maps unmapped `decimal` EF properties to `numeric` without explicit precision/scale, which defaults to arbitrary precision — the column can store values with up to 131072 digits before the decimal point and 16383 after. While this does not introduce truncation, it means: (1) there is no column-level constraint preventing astronomically large or small financial values (e.g. `999999999999999999.9999999999`). (2) Application behaviour and database storage precision are decoupled — a future EF migration on a different provider may infer a different default. (3) Reporting queries that aggregate these columns may produce inconsistent rounding across different execution paths. |
| **Priority** | Medium |
| **Recommendation** | Supply explicit precision hints in `MoneyValueConverter`: `new ConverterMappingHints(precision: 18, scale: 4)`. This propagates to all EF-mapped `Money` columns automatically, creating `numeric(18,4)` columns — a standard choice for financial data (supports values up to approximately 100 trillion with 4 decimal places). Verify the migration reflects the intended column type. |

---

### 4. `MoneyValueConverter` Applies No Validation During Read — Silently Accepts DB Values Outside Domain Constraints

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs](FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs#L14) |
| **Lines** | [L14–L17](FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs#L14) |
| **Description** | The `fromDb` conversion is `decimalInstance => decimalInstance` — it creates a `Money` from any `decimal` value without validation. If a negative value is inserted into the database (e.g. via a direct SQL statement, a seeder, or a future code path that bypasses the command handlers), it will be silently returned as a `Money` with a negative value to every query that reads it. Combined with the absence of non-negativity guards in the `Money` constructor (Domain audit finding 4), the converter represents the last potential enforcement point — and it enforces nothing. |
| **Priority** | Medium |
| **Recommendation** | Consider whether the converter is an appropriate validation layer (it is not, for non-zero contexts — that belongs in the domain). Instead, add database `CHECK` constraints via EF configuration where non-negativity is a business rule (e.g. `builder.ToTable(t => t.HasCheckConstraint("CK_Fund_Amount_NonNegative", "\"Amount\" >= 0"))` on `FundConfiguration`). This provides defence-in-depth at the DB layer regardless of application code paths. |

---

### 5. `Subscription.Price` Column Has a Unique Index — Financial Amount as Unique Constraint

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/Configurations/SubscriptionConfiguration.cs](FinanceBackEnd/src/Finance.Persistence/Configurations/SubscriptionConfiguration.cs#L27) |
| **Lines** | [L27–L29](FinanceBackEnd/src/Finance.Persistence/Configurations/SubscriptionConfiguration.cs#L27) |
| **Description** | `SubscriptionConfiguration` adds `.HasIndex(o => o.Price).IsUnique()`. This means no two subscriptions can have the same price. This is a domain integrity violation: in practice, multiple subscriptions from different services can have identical prices (e.g., two services both priced at $9.99/month). A user attempting to add a second subscription at an existing price will receive an opaque database constraint violation (typically a `DbUpdateException` wrapping a PostgreSQL unique violation), rather than a meaningful validation error. This may also be a data-model bug masquerading as a security constraint. |
| **Priority** | Medium |
| **Recommendation** | Remove the unique index on `Price` unless the domain truly prohibits duplicate prices. If the intent was to prevent completely duplicate subscriptions, index the combination of `(Name, Price, CurrencyId, Frequency)` instead, which expresses the actual uniqueness invariant. |

---

### 6. `AutoCreateOwnershipPermissions()` Synchronous Overload Uses Blocking Synchronous DB Call

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs](FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs#L143) |
| **Lines** | [L143–L158](FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs#L143) |
| **Description** | The synchronous `AutoCreateOwnershipPermissions()` called from `SaveChanges()` performs `.FirstOrDefault()` — a blocking synchronous EF Core query — on the `User` set inside a `DbContext` that is already in the middle of `SaveChanges`. Synchronous EF Core operations block the thread-pool thread for the full round-trip. While ASP.NET Core's pipeline typically uses `SaveChangesAsync`, any code path that calls the synchronous `SaveChanges()` will block a thread-pool thread during the user lookup. Additionally, `DbContext` is not designed to be re-entered during `SaveChanges`; the user lookup initiates a new read while changes are pending, which can cause unexpected state in some EF scenarios. |
| **Priority** | Medium |
| **Recommendation** | Remove the synchronous `SaveChanges()` override and `AutoCreateOwnershipPermissions()` synchronous overload. Make `SaveChangesAsync` the only supported code path for application code. Mark the synchronous `SaveChanges()` as `[Obsolete]` or throw `NotSupportedException` to enforce async usage, which is the standard guidance for ASP.NET Core EF Core contexts. |

---

### 7. `FinanceDbContext.CurrentUserId` Uses `Identity.Name` Claim — Diverges from Auth Module's `ClaimTypes.NameIdentifier`

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs](FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs#L67) |
| **Lines** | [L67](FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs#L67) |
| **Description** | `CurrentUserId` reads `User.Identity.Name`. In `AuthenticationExtensions.cs`, the JWT configuration sets `NameClaimType = ClaimTypes.NameIdentifier`, which maps the Auth0 `sub` claim to `ClaimTypes.NameIdentifier`. ASP.NET Core resolves `IIdentity.Name` from `ClaimTypes.Name` by default, not `ClaimTypes.NameIdentifier`. The two claim types are different. If `ClaimTypes.Name` is absent from the Auth0 token (which it is by default for Auth0 machine-to-machine tokens and API JWTs that do not include a `name` claim), `User.Identity.Name` will be `null`, making `CurrentUserId` the fallback sentinel `"IdentityNotFound"` — silently bypassing all ownership filters even for fully authenticated requests that have a valid `sub` claim. The correct resolution is `User.FindFirstValue(ClaimTypes.NameIdentifier)`, which reads the explicitly configured claim. |
| **Priority** | Critical |
| **Recommendation** | Replace `HttpContextAccessor?.HttpContext?.User?.Identity?.Name` with `HttpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value`. This aligns the query filter's identity lookup with the JWT configuration in `AuthenticationExtensions` and ensures ownership filters use the same claim as the authorization policies. Verify with an integration test that an authenticated request returns the correct `sub` value from this path. |
