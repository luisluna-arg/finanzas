# Security Audit — Finance.Application Queries

**Date**: 2026-03-29
**Scope**: `Finance.Application/Queries/**`, `Finance.Application/Dtos/**`, `Finance.Api/Controllers/Queries/**`
**Reviewer**: GitHub Copilot

---

## Findings

### 1. BOLA — `GetAllUsersQuery` Returns All Users to Any Authenticated Caller

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/Users/GetAllUsersQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/Users/GetAllUsersQuery.cs#L14) |
| **Lines** | [L14–L30](FinanceBackEnd/src/Finance.Application/Queries/Users/GetAllUsersQuery.cs#L14) |
| **Description** | `GetAllUsersQueryHandler` queries `DbContext.User` with no ownership filter. The `User` table has no EF global query filter (only financial entities like `Movement`, `Fund`, `Income`, etc. have ownership filters applied via `ModelBuilderExtensions`). Any authenticated user hitting `GET /api/users` receives the full user list, including usernames, first/last names, roles, and their linked identity records. This allows cross-user enumeration of the entire user base. |
| **Priority** | Critical |
| **Recommendation** | Unless an admin-only endpoint is intended, restrict `GetAllUsersQuery` to only return the caller's own record. If the full list is required for admin scenarios, guard it behind a role policy (e.g. `RequireRole("Admin")`) and separate it from the owner-accessible endpoint. |

---

### 2. BOLA — `GetUserByIdQuery` Accepts Any User ID Without Ownership Check

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/Users/GetUserByIdQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/Users/GetUserByIdQuery.cs#L13) |
| **Lines** | [L13–L17](FinanceBackEnd/src/Finance.Application/Queries/Users/GetUserByIdQuery.cs#L13) |
| **Description** | `GetUserByIdQueryHandler` fetches any `User` by the internal GUID supplied in the request, with no check that the requested ID belongs to the currently authenticated user. Combined with `UserQueryController` at `GET /api/users/{id}`, any user with a valid token can retrieve the profile, roles, and Auth0 identity source IDs of any other user in the system by guessing or enumerating their internal GUID. |
| **Priority** | Critical |
| **Recommendation** | In the handler, verify that `request.Id == authenticatedUserId` (resolved from the JWT claim) before returning data. For admin lookups, route through a separate query guarded by an admin-only policy. |

---

### 3. BOLA — `GetAllIdentitiesQuery` Exposes All Auth0 Source IDs to Any Authenticated User

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/Identities/GetAllIdentitiesQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/Identities/GetAllIdentitiesQuery.cs#L15) |
| **Lines** | [L15–L27](FinanceBackEnd/src/Finance.Application/Queries/Identities/GetAllIdentitiesQuery.cs#L15) |
| **Description** | `GetAllIdentitiesQueryHandler` queries `DbContext.Identity` with no ownership restriction. The `Identity` table contains `SourceId` (the Auth0 `sub` / user_id claim) for every user in the system. `GET /api/identities` is exposed via `IdentityQueryController`, which inherits `SecuredApiController` (`AdminOrOwnerPolicy`), but that policy only verifies the caller is an authenticated owner — it does not scope results to the caller's own records. Any valid token holder can enumerate every Auth0 identity in the database. |
| **Priority** | Critical |
| **Recommendation** | Filter the query to identities belonging to the authenticated user: `.Where(i => i.User.Identities.Any(i2 => i2.SourceId == currentUserSourceId))`. If admin-level enumeration is needed, gate it behind a dedicated admin policy on a separate endpoint. |

---

### 4. BOLA — `GetIdentityQuery` (by ID) Has No Ownership Check

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/Identities/GetIdentityQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/Identities/GetIdentityQuery.cs#L14) |
| **Lines** | [L14–L15](FinanceBackEnd/src/Finance.Application/Queries/Identities/GetIdentityQuery.cs#L14) |
| **Description** | `GetIdentityQueryHandler` retrieves an `Identity` record by its internal GUID with no ownership verification. `GET /api/identities/{id}` allows any authenticated caller to retrieve the Auth0 source ID and provider details of any identity record by its primary key. |
| **Priority** | Critical |
| **Recommendation** | Add a caller ownership check before returning the record: verify that the requested identity's `UserId` matches the authenticated user's internal database ID. |

---

### 5. BOLA — `GetIdentitiesQuery` Accepts Arbitrary `UserId` Without Ownership Validation

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/Users/GetIdentitiesQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/Users/GetIdentitiesQuery.cs#L12) |
| **Lines** | [L12](FinanceBackEnd/src/Finance.Application/Queries/Users/GetIdentitiesQuery.cs#L12), [L20](FinanceBackEnd/src/Finance.Application/Queries/Users/GetIdentitiesQuery.cs#L20) |
| **Description** | `GetIdentitiesQuery` takes `UserId` (a `Guid`) from the request and filters `DbContext.Identity` by that value without verifying the supplied `UserId` matches the authenticated caller. A caller who knows or can guess another user's GUID can retrieve that user's full identity list, including their Auth0 `SourceId`. |
| **Priority** | High |
| **Recommendation** | Remove the caller-supplied `UserId` property. Resolve the authenticated user's internal ID from `IHttpContextAccessor` or the dispatch context and use that as the filter exclusively. |

---

### 6. `IdentityDto` Serializes Auth0 `SourceId` to API Response

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Dtos/Identities/IdentityDto.cs](FinanceBackEnd/src/Finance.Application/Dtos/Identities/IdentityDto.cs#L9) |
| **Lines** | [L9–L10](FinanceBackEnd/src/Finance.Application/Dtos/Identities/IdentityDto.cs#L9) |
| **Description** | `IdentityDto` exposes both `SourceId` (the raw Auth0 `sub` claim, e.g. `auth0|64abc...`) and `UserId` (internal database GUID) in the API response. Even if access is restricted to the caller's own records (after fixing findings 3–5), the `SourceId` is a sensitive identifier that can be used to correlate a user across systems. It should not be surfaced as a queryable API field. |
| **Priority** | High |
| **Recommendation** | Remove `SourceId` from `IdentityDto`. Expose only the `Provider` type and a non-reversible indicator (e.g. a hashed or masked identifier) if the frontend needs to display linked accounts. |

---

### 7. Unbounded `PageSize` on All Paginated Financial Queries

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/_Base/GetPaginatedQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/_Base/GetPaginatedQuery.cs#L17) |
| **Lines** | [L17–L18](FinanceBackEnd/src/Finance.Application/Queries/_Base/GetPaginatedQuery.cs#L17) |
| **Description** | The base `GetPaginatedQuery<T>` declares `Page` and `PageSize` as plain `int` with no maximum enforcement. All paginated query handlers (`GetPaginatedMovementsQuery`, `GetPaginatedDebitsQuery`, `GetPaginatedCreditCardTransactionsQuery`, `GetPaginatedCreditCardStatementsQuery`, `GetPaginatedIOLInvestmentsQuery`, `GetPaginatedSubscriptionsQuery`) pass these values directly to `.Skip()` and `.Take()` with no cap. A caller can set `PageSize = int.MaxValue` to force a full table dump in a single HTTP response, bypassing the intended pagination. Additionally, `GetPaginatedIncomesQuery` ([L63](FinanceBackEnd/src/Finance.Application/Queries/Incomes/GetPaginatedIncomesQuery.cs#L63)) falls back to `totalItems` when `PageSize = 0`, explicitly returning all records for that input. |
| **Priority** | High |
| **Recommendation** | Enforce a maximum page size in the base class (e.g. `public int PageSize { get => Math.Min(_pageSize, 500); set => _pageSize = value; }`). Remove the `PageSize = 0 → totalItems` fallback or make it admin-only. |

---

### 8. `GetUserBySourceIdsQuery` — Arbitrary Source ID Lookup, No Self-Restriction

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/Users/GetUserBySourceIdsQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/Users/GetUserBySourceIdsQuery.cs#L12) |
| **Lines** | [L12–L34](FinanceBackEnd/src/Finance.Application/Queries/Users/GetUserBySourceIdsQuery.cs#L12) |
| **Description** | `GetUserBySourceIdsQuery` accepts a caller-supplied `string[]` of Auth0 source IDs and returns the matching user with their roles and identity records. The handler performs no validation that the requested source IDs match the authenticated caller's own `sub` claim. While its current callers (`SessionController`, `UserQueryController`) only pass the authenticated user's own sourceId, the query itself is a generic lookup that any internal code path can invoke with arbitrary IDs, returning any user's profile. |
| **Priority** | Medium |
| **Recommendation** | Either make the query internal-only (not dispatchable from untrusted inputs) or add an assertion in the handler that the requested source IDs are a subset of the caller's own identities. |

---

### 9. `GetPaginatedMovementsQuery` — String GUIDs Parsed Without Input Validation

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Queries/Movements/GetPaginatedMovementsQuery.cs](FinanceBackEnd/src/Finance.Application/Queries/Movements/GetPaginatedMovementsQuery.cs#L43) |
| **Lines** | [L43](FinanceBackEnd/src/Finance.Application/Queries/Movements/GetPaginatedMovementsQuery.cs#L43), [L48](FinanceBackEnd/src/Finance.Application/Queries/Movements/GetPaginatedMovementsQuery.cs#L48) |
| **Description** | `AppModuleId` and `BankId` are stored as raw `string?` properties and parsed inline via `Guid.Parse(request.AppModuleId)`. `Guid.Parse` throws `FormatException` for invalid input, which propagates as an unhandled 500. The same pattern is repeated in `GetPaginatedCreditCardStatementsQuery` ([L47](FinanceBackEnd/src/Finance.Application/Queries/CreditCards/GetPaginatedCreditCardStatementsQuery.cs#L47)), `GetPaginatedCreditCardTransactionsQuery` ([L52](FinanceBackEnd/src/Finance.Application/Queries/CreditCards/GetPaginatedCreditCardTransactionsQuery.cs#L52)), and `GetPaginatedIOLInvestmentsQuery` ([L44](FinanceBackEnd/src/Finance.Application/Queries/IOLInvestments/GetPaginatedIOLInvestmentsQuery.cs#L44)). |
| **Priority** | Medium |
| **Recommendation** | Change the property types to `Guid?` and perform the parsing at the controller boundary using `[FromQuery] Guid?`. Return HTTP 400 for malformed GUIDs at the controller level before dispatching the query. |

---

### 10. `FinanceDispatchContextBuilder` Throws Uncaught `InvalidOperationException` for Unknown Users

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Auth/FinanceDispatchContextBuilder.cs](FinanceBackEnd/src/Finance.Application/Auth/FinanceDispatchContextBuilder.cs#L31) |
| **Lines** | [L31–L34](FinanceBackEnd/src/Finance.Application/Auth/FinanceDispatchContextBuilder.cs#L31) |
| **Description** | When the authenticated JWT's `sub` claim does not match any identity in the local database, `FinanceDispatchContextBuilder.BuildAsync` throws `InvalidOperationException("Context User Info is null...")`. This exception is unhandled and results in an HTTP 500 response. In development mode ASP.NET Core may include the stack trace and exception message in the body. The scenario is realistic when a user is created in Auth0 but the local user seeding has not yet run, or during a partial rollout. |
| **Priority** | Medium |
| **Recommendation** | Catch this specific case in the builder and return a structured `401 Unauthorized` or `403 Forbidden` response via problem details middleware rather than allowing the exception to propagate uncaught. Register a global exception handler (`app.UseExceptionHandler`) to ensure no unhandled exception ever returns a 500 with internal details. |
