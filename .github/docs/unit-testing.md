# Unit Testing in Finanzas Backend

## Project

All backend unit tests live in:

```
FinanceBackEnd/tests/Finance.Application.Tests/
```

Test runner: **xUnit**. Mocking: **Moq**. Global usings (`Xunit`, `Moq`) are declared in `Usings.cs` — no need to re-import them.

---

## Folder Structure

Tests mirror the `Finance.Application` source layout:

```
Tests/
  Commands/
    CreditCards/
    Funds/
    IOLInvestments/
    ...
    _Base/           ← ActivateDeactivateTestBase (activate/deactivate helpers)
  Queries/
    Currencies/
    Funds/
    Summary/
    ...
    _Base/           ← QueryHandlerBaseTests (in-memory DbContext)
  Services/
    CurrencyConversionServiceTests.cs
    ...
  Domain/
    DataConverters/
    ...
```

Rule: one test class per handler/service, in the matching subfolder.

---

## Base Classes

### `QueryHandlerBaseTests`

**File**: `Queries/_Base/QueryHandlerBaseTests.cs`  
**Namespace**: `Finance.Application.Tests.Queries.Base`

Use this for **any** test that needs `FinanceDbContext`:

- Command handler tests
- Query handler tests
- Service tests that take `FinanceDbContext`

```csharp
public class MyQueryHandlerTests : QueryHandlerBaseTests
{
    // _dbContext is available
}
```

`Dispose()` is `virtual` — override it when the test class holds its own disposable (e.g. `IMemoryCache`):

```csharp
public override void Dispose()
{
    base.Dispose();
    _cache.Dispose();
}
```

### `ActivateDeactivateTestBase`

**File**: `Commands/_Base/ActivateDeactivateTestBase.cs`  
**Namespace**: `Finance.Application.Tests.Commands.Base`

Use when testing activate/deactivate command handlers. Provides `DbContext` + a pre-seeded `CurrentUser`.

---

## The In-Memory Database

`QueryHandlerBaseTests` creates a fresh `FinanceDbContext` backed by `UseInMemoryDatabase` with a random name per test class instance. Each test class gets its own isolated database.

### `CurrentUserId`

`FinanceDbContext.CurrentUserId` returns `"IdentityNotFound"` when no `IHttpContextAccessor` is provided (which is always the case in tests). To match the current user in query filters, seed a user whose `Identity.SourceId == "IdentityNotFound"`:

```csharp
var user = new User
{
    Id = Guid.NewGuid(),
    Username = "u",
    FirstName = "F",
    LastName = "L",
    Identities = [new Identity { SourceId = "IdentityNotFound" }],
};
await _dbContext.User.AddAsync(user);
await _dbContext.SaveChangesAsync();
```

---

## EF Core Query Filters

Several entities have **global query filters** that restrict reads to owned records. Tests must seed the corresponding `*Permissions` records or the entity will not appear in query results.

| Entity | Permissions Table |
|---|---|
| `Fund` | `FundPermissions` |
| `CurrencyExchangeRate` | `CurrencyExchangeRatePermissions` |
| `IOLInvestment` | `IOLInvestmentPermissions` |
| `IOLInvestmentAsset` | `IOLInvestmentAssetPermissions` |

Always seed **both** the entity and its permission record. Example for `Fund`:

```csharp
var fund = new Fund { Id = Guid.NewGuid(), ... };
await _dbContext.Fund.AddAsync(fund);
_dbContext.FundPermissions.Add(new FundPermissions
{
    ResourceId = fund.Id,
    Resource = fund,
    UserId = user.Id,
    User = user,
    PermissionLevels = [PermissionLevelEnum.Owner],
});
await _dbContext.SaveChangesAsync();
```

To query data bypassing filters (e.g. to seed permissions for all records at once):

```csharp
var all = _dbContext.Fund.IgnoreQueryFilters().ToArray();
```

`IgnoreQueryFilters` requires `using Microsoft.EntityFrameworkCore;` when not already present.

---

## Mocking the Dispatcher

When a handler or service calls `IDispatcher<FinanceDispatchContext>`, inject a `Mock<IDispatcher<FinanceDispatchContext>>` and set it up by return type:

```csharp
_dispatcher
    .Setup(d => d.DispatchQueryAsync<List<CurrencyExchangeRate>>(It.IsAny<IQuery<List<CurrencyExchangeRate>>>()))
    .ReturnsAsync(DataResult<List<CurrencyExchangeRate>>.Success(rates));
```

Verify call count when the behavior matters (e.g. cache suppresses a second dispatch):

```csharp
_dispatcher.Verify(
    d => d.DispatchQueryAsync<List<CurrencyExchangeRate>>(It.IsAny<IQuery<List<CurrencyExchangeRate>>>()),
    Times.Once);
```

### `CurrencyConversionService` — same-currency behavior

- `Convert(holder, targetId)`: **short-circuits** if `holder.CurrencyId == targetId` (no dispatcher call).
- `ConvertCollection(holders, targetId)`: **always** calls the dispatcher (no short-circuit). Set up rates even when the amounts are in the same currency.

---

## Building and Running Tests

Always build before running to catch compile errors early:

```powershell
dotnet build FinanceBackEnd/tests/Finance.Application.Tests/Finance.Application.Tests.csproj
```

Run all tests:

```powershell
dotnet test FinanceBackEnd/tests/Finance.Application.Tests/Finance.Application.Tests.csproj -v q
```

Run a single test class:

```powershell
dotnet test FinanceBackEnd/tests/Finance.Application.Tests/ --filter "FullyQualifiedName~GetCurrentFundsQueryHandlerTests" -v q
```

---

## Pre-Seeded Data

`EnsureCreated()` runs all `IEntityTypeConfiguration` classes, which include `HasData` seed calls. The following data is always present in every test DB — **never try to insert it again**:

| Data | Source |
|---|---|
| `Currency` ARS (ID `6d189135-7040-45a1-b713-b1aa6cad1720`) | `CurrencyConfiguration.HasData` |
| `Currency` USD (ID `efbf50bc-...`) | `CurrencyConfiguration.HasData` |
| `CurrencySymbol` records for ARS and USD | `CurrencySymbolConfiguration.HasData` |
| All `FrequencyEnum` values (`Monthly`, `Annual`, `Weekly`, `Daily`, `OneTime`) | `KeyValueEntityConfiguration<Frequency, FrequencyEnum>` |
| All `IOLInvestmentAssetTypeEnum` values (incl. `Cedear`) | `KeyValueEntityConfiguration<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum>` |
| All `AppModuleTypeEnum` values (incl. `Debits`, `Funds`, `Investments`) | `KeyValueEntityConfiguration<AppModuleType, AppModuleTypeEnum>` |

### Inserting pre-seeded entities causes a duplicate-key crash

```
System.ArgumentException: An item with the same key has already been added. Key: <id>
```

**Never** do:
```csharp
// ❌ crashes — ARS is already seeded with this exact ID
var ars = new Currency { Id = Guid.Parse(CurrencyConstants.DefaultCurrencyId), ShortName = "ARS" };
await _dbContext.Currency.AddAsync(ars);

// ❌ crashes — IOLInvestmentAssetType.Cedear is already seeded
var type = new IOLInvestmentAssetType { Id = IOLInvestmentAssetTypeEnum.Cedear };
await _dbContext.IOLInvestmentAssetType.AddAsync(type);
```

**Instead, fetch the existing entity:**
```csharp
// ✅ fetch pre-seeded currency
var ars = (await _dbContext.Currency.FindAsync(Guid.Parse(CurrencyConstants.DefaultCurrencyId)))!;

// ✅ fetch pre-seeded key-value entity
var assetType = _dbContext.IOLInvestmentAssetType.First(o => o.Id == IOLInvestmentAssetTypeEnum.Cedear);
var appModuleType = _dbContext.AppModuleType.Find(AppModuleTypeEnum.Debits)!;
```

### Asserting counts or ordering when pre-seeded data is present

Tests that call `Assert.Single(result.Data)` or `Assert.Equal(N, result.Data.Count)` will fail if the query returns pre-seeded records too. Scope assertions to the IDs the test controls:

```csharp
// ❌ fragile — pre-seeded currencies inflate the count
Assert.Equal(1, result.Data.Count);

// ✅ scope to the test's own records
var mine = result.Data.Where(x => x.Id == myId || x.Id == otherId).ToList();
Assert.Single(mine);
```

---

## `CurrencyFixture`

**File**: `Queries/_Base/CurrencyFixture.cs`  
**Namespace**: `Finance.Application.Tests.Queries.Base`

Builds `Currency` instances with short names guaranteed not to clash with pre-seeded ARS/USD. Use it in any test that needs to insert a currency into the DB:

```csharp
// single currency with auto-assigned short name
var c = CurrencyFixture.Build();

// with explicit attributes
var inactive = CurrencyFixture.Build(shortName: "JPY", name: "Yen", deactivated: true);

// batch
var currencies = CurrencyFixture.BuildMany(3);
```

Available non-seeded short names (rotated automatically): EUR, GBP, JPY, CHF, CAD, BRL, MXN, CNY, KRW, INR, RUB, TRY, ZAR, SEK, NOK, DKK, PLN, CZK, HUF, RON.

---

## Gotchas

| Symptom | Cause | Fix |
|---|---|---|
| `An item with the same key has already been added` | Inserting an entity whose ID matches a pre-seeded record | Fetch from `_dbContext` instead of constructing with `new` |
| `Assert.Single` / count assertion fails unexpectedly | Pre-seeded rows are included in query results | Scope assertions by the IDs the test seeded |
| Query returns empty collection | Missing `*Permissions` seed | Add the matching `*Permissions` record for the test user |
| `IOLInvestment` query always empty even with permissions | `IOLInvestmentAsset` **also** has a query filter | Grant `IOLInvestmentAssetPermissions` for each asset too |
| `NullReferenceException` in `ConvertCollection` | No dispatcher setup for same-currency call | `SetupDispatcherRates([])` even when all amounts share the same currency |
| `IgnoreQueryFilters`/`CountAsync` not found | Missing `using Microsoft.EntityFrameworkCore;` | Add the using |
| Test user not matched by query filter | `Identity.SourceId` ≠ `"IdentityNotFound"` | Use `SourceId = "IdentityNotFound"` for the test user identity |
| Handler returns failure saying "Default currency not found" | Test expected ARS to be absent — impossible since it's pre-seeded | Assert success instead; the default currency is always present |
