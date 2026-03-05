# Context Propagation — Architecture Rework

## Problem

The current implementation uses an `HttpRequest?` parameter that gets passed down through multiple layers (saga services → orchestrators → dispatcher) solely to hydrate a `FinanceDispatchContext` containing the authenticated user. This is necessary because several commands (`CreateResourcePermissionsCommand`, `DeleteEntityOwnerCommand`, etc.) are `IContextAwareCommand` and need to know *who* is executing the operation.

### What's wrong

1. **`HttpRequest` leaks into the application layer.** Orchestrators and saga services have no business knowing about HTTP. They're supposed to be infrastructure-agnostic.

2. **Easy to forget — and silent failures.** Calling `Dispatcher.DispatchAsync(command)` instead of `Dispatcher.DispatchAsync(command, httpRequest)` compiles fine but silently uses `UserInfo = { Id = Guid.Empty }`, causing:
   - Permission creation that fails to find the user → returns `"User not found"` (400 error, the original bug).
   - Permission deletion that matches nothing → silent no-op.

3. **`UserId!.Value` null-reference crashes.** When a `SetXxxOwnerSagaRequest` is constructed with only an `id` (the common case during creation flows), `UserId` is null, and the `!.Value` dereference throws `InvalidOperationException: Nullable object must have a value`.

4. **The context has two identities.** `FinanceDispatchContext.UserInfo` (the DB `User` entity) is built lazily by `FinanceDispatchContextBuilder` from `HttpRequest`. But `CreateResourcePermissionsCommand` also has its own `UserId?` field as an override path — two ways to express the same thing.

---

## Design Constraint

Resources have ownership — some operations must record or verify *who* is acting. But the vast majority of commands (update amount, change bank, delete a debit, etc.) are pure business logic that has nothing to do with the user. The goal is to make user identity **opt-in, only where ownership matters**, not a global concern threaded through every call.

## Desired Architecture

### Recommended: Ambient scoped `ICurrentUserContext`

Introduce a scoped service populated **once** by middleware from the JWT, before any handler runs:

```csharp
public interface ICurrentUserContext
{
    Guid UserId { get; }
    User UserInfo { get; }
}
```

Commands and handlers that deal with ownership inject it directly. Everything else ignores it entirely.

```
Middleware
  └── populates ICurrentUserContext from JWT (once per request)

Controller
  └── calls SagaService / Dispatcher — no user data in method signatures

CreateResourcePermissionsCommandHandler(ICurrentUserContext currentUser)
  └── uses currentUser.UserId directly — no Context, no HttpRequest

DeleteEntityOwnerCommandHandler(ICurrentUserContext currentUser)
  └── uses currentUser.UserId directly

UpdateFundCommandHandler, CreateDebitCommandHandler, etc.
  └── don't inject ICurrentUserContext at all — unaffected
```

**Benefits:**
- Zero changes to the 90% of commands that don't touch ownership.
- Ownership commands are explicit about their dependency via constructor injection.
- `HttpRequest`, `IContextAwareCommand`, `FinanceDispatchContext`, and `DispatchContextBuilder` can all be removed.
- `SetXxxOwnerSagaRequest(id)` constructor (without userId) remains valid — the handler resolves the user itself.
- No user data pollutes saga service / orchestrator method signatures.

### What to remove

| Current | Replace with |
|---|---|
| `IContextAwareCommand<TContext, TResult>` | Plain `ICommand<TResult>` |
| `FinanceDispatchContext` / `DispatchContextBuilder` | `ICurrentUserContext` scoped service |
| `HttpRequest?` params on orchestrators / saga services | Nothing — remove entirely |
| `command.Context.UserInfo.Id` in handlers | `_currentUser.UserId` injected in constructor |
| `Dispatcher.DispatchAsync(cmd, httpRequest)` overload | Single `Dispatcher.DispatchAsync(cmd)` |

---

## Affected Files (as of this writing)

| File | Issue |
|---|---|
| `Services/Orchestrators/FundPermissionsOrchestrations/OrchestrateSet.cs` | Was crashing on `UserId!.Value`, missing `httpRequest` on dispatch |
| `Services/Orchestrators/FundPermissionsOrchestrations/OrchestrateDelete.cs` | Missing `httpRequest` → silent deletion no-op |
| `Services/Orchestrators/SubscriptionPermissionsOrchestrations/OrchestrateSet.cs` | Same as fund set |
| `Services/Orchestrators/SubscriptionPermissionsOrchestrations/OrchestrateDelete.cs` | Same as fund delete |
| `Services/Orchestrators/CurrencyExchangeRateOrchestrations/OrchestrateSet.cs` | Missing `httpRequest` on dispatch |
| `Commands/_Base/CreateResourcePermissionsCommand.cs` | Dual userId resolution (explicit `UserId` vs `Context.UserIdClaim`) |
| `Commands/_Base/DeleteEntityOwnerCommand.cs` | Uses `command.Context.UserInfo.Id` — breaks silently with empty context |
| `CQRSDispatch/Dispatcher.cs` | `BuildAsync(httpRequest)` called deep inside dispatch — HTTP concern in dispatch layer |

---

## Short-term Fix Applied

- Removed `UserId!.Value` null dereferences; pass `null` and let the handler fall back to `Context.UserIdClaim`.
- Pass `httpRequest` through to all `DispatchAsync` calls on context-aware commands in orchestrators.

This is a band-aid. The real fix is one of the options above.
