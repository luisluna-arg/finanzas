# Orchestration Layer — Simplification Plan

## Entities with permissions

All entities that have a `ResourcePermissions` model in the domain. The **Orchestrated** column indicates whether the saga + orchestrator stack is currently wired up for that entity.

| Entity | Permissions model | Orchestrated |
|---|---|---|
| `Fund` | `FundPermissions` | ✅ |
| `Income` | `IncomePermissions` | ✅ |
| `CurrencyExchangeRate` | `CurrencyExchangeRatePermissions` | ✅ |
| `Subscription` | `SubscriptionPermissions` | ✅ |
| `CreditCard` | `CreditCardPermissions` | ❌ |
| `Debit` | `DebitPermissions` | ❌ |
| `DebitOrigin` | `DebitOriginPermissions` | ❌ |
| `IOLInvestment` | `IOLInvestmentPermissions` | ❌ |
| `IOLInvestmentAsset` | `IOLInvestmentAssetPermissions` | ❌ |
| `Movement` | `MovementPermissions` | ❌ |

---

## Current Structure (per resource, e.g. Fund)

```
Controller
  └── FundService  (BaseResourceSagaService<Fund, FundPermissions, FundOrchestrator,
      │              FundPermissionsOrchestrator, CreateFundSagaRequest, UpdateFundSagaRequest,
      │              DeleteFundSagaRequest, SetFundOwnerSagaRequest, DataResult<FundPermissions>,
      │              DeleteFundOwnerSagaRequest, CommandResult>)
      │   manages transaction, calls orchestrator
      │
      ├── FundOrchestrator  (partial classes: OrchestrateCreation, OrchestrateUpdate, OrchestrateDelete)
      │    └── dispatches CreateFundCommand, UpdateFundCommand, DeleteFundCommand
      │    └── calls FundOwnerService
      │
      └── FundOwnerService  (BaseResourcePermissionsSagaService<...>)
           manages transaction, calls permissions orchestrator
           └── FundPermissionsOrchestrator  (partial classes: OrchestrateSet, OrchestrateDelete)
                └── dispatches CreateFundPermissionsCommand, DeleteFundOwnerCommand
```

Each resource also has a parallel set of **SagaRequest** types that mirror the Command properties:
- `CreateFundSagaRequest` → wraps same data as `CreateFundCommand`
- `SetFundOwnerSagaRequest` → wraps `id` + `UserId?`

### File count per entity (current)

| Layer | Files |
|---|---|
| CRUD commands + handlers | 3 |
| Owner commands + handlers | 2 |
| `XxxOrchestrations/` partial files | 3 |
| `XxxPermissionsOrchestrations/` partial files | 2 |
| SagaRequest classes (Create / Update / Delete) | 3 |
| Owner SagaRequest classes (Set / Delete) | 2 |
| SagaRequest base classes | 2 |
| `XxxService.cs` | 1 |
| `XxxOwnerService.cs` | 1 |
| **Total per entity** | **19** |

Plus 4 shared base classes with 6–11 generic type parameters each.

### Problems

1. **Generic parameter explosion.** `BaseResourceSagaService` and `BaseResourceOrchestrator` take 10–11 type parameters each. Adding a new resource requires copy-pasting the entire type chain.

2. **Two objects for one concept.** `CreateFundSagaRequest` is a subclass of `CreateFundCommand` via `UpsertFundSagaRequestBase`. That base class constructor takes the same fields the command already has, and copies them back in. The orchestrator then unpacks the request into a *new* `CreateFundCommand` before dispatching. The SagaRequest is a field-copy of the command it inherits from.

3. **Four abstractions for one flow.** Create/update/delete for an owned resource goes through: `SagaRequest → SagaService → Orchestrator → Command`. This is three layers of indirection for what is typically 5–10 lines of logic.

4. **Permissions are a separate parallel hierarchy.** `FundService` and `FundOwnerService` are always used together, always manage the same resource, but live in separate class trees. The "create fund + assign ownership" flow crosses both and requires the `FundOrchestrator` to call back into `FundOwnerService`.

5. **Partial class orchestrators.** Each CRUD operation is a separate file (`OrchestrateCreation.cs`, `OrchestrateUpdate.cs`, `OrchestrateDelete.cs`). The partials add file count without improving clarity.

---

## Proposed Structure

Collapse the four abstraction layers into **one service per resource**, with direct command dispatch and inline ownership handling.

```
Controller
  └── FundService
       ├── Create(CreateFundRequest) → builds command, dispatches command + ownership command in one transaction
       ├── Update(UpdateFundRequest) → builds command, dispatches command
       └── Delete(DeleteFundRequest) → builds command, dispatches command + removes ownership in one transaction
```

### Layer diagram

```
  HTTP Request
       │
       ▼
  ┌─────────────┐
  │ Controller  │
  └──────┬──────┘
         │ CreateFundRequest (sealed record)
         ▼
  ┌──────────────────────────────────────────────────┐
  │ FundService                  [transaction scope] │
  │                                                  │
  │  1. new CreateFundCommand(...)         ──────────┼──► CreateFundCommandHandler
  │  2. new CreateFundPermissionsCommand() ──────────┼──► CreateFundPermissionsCommandHandler
  └──────────────────────────────────────────────────┘
                                                            │
                                                            ▼
                                                       DbContext / Domain
```

Commands never cross back up the stack. Each handler owns its own aggregate. The service is the only place that knows about sequencing and the transaction scope.

---

### Key changes

**1. Replace SagaRequest types with sealed requests.**
Controllers pass sealed request records. Services construct commands internally. Requests are flat, no inheritance, no base classes, no generics — just the data the operation needs. The old `CreateFundSagaRequest` subclassed `CreateFundCommand` and copied fields through a base constructor; the new `CreateFundRequest` is a plain `sealed record` with the same properties and no ceremony.

**2. Remove Orchestrators.**
The logic inside `OrchestrateCreation` etc. moves directly into the service method. It's typically 5–15 lines.

**3. Fold ownership into the main service.**
`FundOwnerService` + `FundPermissionsOrchestrator` are eliminated. `FundService` dispatches `CreateFundPermissionsCommand` / `DeleteFundOwnerCommand` directly (and once `ICurrentUserContext` is in place, those commands handle user resolution themselves).

**4. Transactions inline, not a base class concern.**
The transaction wrapping logic (`BeginTransaction → try/commit/rollback`) is written inline in the service method. It's 6 lines, fully visible, and needs no abstraction. This replaces the current approach where transaction management is buried in a base class with 10 type parameters.

### What it looks like

```csharp
// Sealed request records — flat, no inheritance, no generics
public sealed record CreateFundRequest(string Name, ...);
public sealed record UpdateFundRequest(Guid Id, string Name, ...);
public sealed record DeleteFundRequest(Guid Id);

public class FundService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
{
    public async Task<DataResult<Fund>> Create(CreateFundRequest request)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var fundResult = await dispatcher.DispatchAsync(
                new CreateFundCommand { Name = request.Name, ... });
            if (!fundResult.IsSuccess) return fundResult;

            await dispatcher.DispatchAsync(
                new CreateFundPermissionsCommand { ResourceId = fundResult.Data.Id });

            await tx.CommitAsync();
            return fundResult;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return DataResult<Fund>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<Fund>> Update(UpdateFundRequest request)
        => await dispatcher.DispatchAsync(new UpdateFundCommand { Id = request.Id, Name = request.Name, ... });

    public async Task Delete(DeleteFundRequest request)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dispatcher.DispatchAsync(new DeleteFundCommand { Id = request.Id });
            await dispatcher.DispatchAsync(new DeleteFundOwnerCommand { EntityId = request.Id });
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
```

No base class. No generics. No orchestrator layer. No separate permissions service. The ownership concern is visible inline where it actually happens.

---

## Migration Path

This doesn't need to be done all at once. Resources can be migrated one at a time. The 4 already-orchestrated entities should be refactored first (collapse the layers without changing behaviour), then the 6 missing ones can be added to the simplified pattern directly — no point wiring them into the current complex architecture first.

**Refactor (already orchestrated, simplify the stack):**
1. `Fund` — largest test surface, safest to prove the pattern
2. `CurrencyExchangeRate`
3. `Subscription`
4. `Income`

**Add (not yet orchestrated, implement directly in simplified pattern):**
5. `Debit`
6. `DebitOrigin`
7. `Movement`
8. `CreditCard`
9. `IOLInvestment`
10. `IOLInvestmentAsset`

Once all 10 are done, delete the entire shared base layer (see below).

## Files to delete (same list applies to all four entities)

Replace `Xxx` / `xxx` with `Fund`, `Income`, `CurrencyExchangeRate`, or `Subscription`:

- `Services/Orchestrators/XxxOrchestrations/OrchestrateCreation.cs`
- `Services/Orchestrators/XxxOrchestrations/OrchestrateUpdate.cs`
- `Services/Orchestrators/XxxOrchestrations/OrchestrateDelete.cs`
- `Services/Orchestrators/XxxPermissionsOrchestrations/OrchestrateSet.cs`
- `Services/Orchestrators/XxxPermissionsOrchestrations/OrchestrateDelete.cs`
- `Services/XxxOwnerService.cs`
- `Services/Requests/Xxx/CreateXxxSagaRequest.cs`
- `Services/Requests/Xxx/UpdateXxxSagaRequest.cs`
- `Services/Requests/Xxx/DeleteXxxSagaRequest.cs`
- `Services/Requests/Xxx/Owners/SetXxxOwnerSagaRequest.cs`
- `Services/Requests/Xxx/Owners/DeleteXxxOwnerSagaRequest.cs`
- `Services/Requests/Xxx/Owners/Base/BaseXxxOwnerSagaRequest.cs`
- `Services/Requests/Xxx/Owners/Base/UpsertXxxSagaRequestBase.cs`

And once all four entities are migrated, delete the entire shared base layer:

- `Services/Base/BaseResourceSagaService.cs`
- `Services/Base/BaseResourceOrchestrator.cs`
- `Services/Base/BaseResourceOwnerOrchestrator.cs`
- `Services/Base/BaseResourceOwnerSagaService.cs`
- `Services/RequestBuilders/IResourceOrchestrator.cs`
- `Services/RequestBuilders/IResourcePermissionsOrchestrator.cs`
- `Services/Interfaces/ISagaRequest.cs`
- `Services/Interfaces/ISagaService.cs`
- `Services/Interfaces/IResourceOwnerSagaService.cs`

