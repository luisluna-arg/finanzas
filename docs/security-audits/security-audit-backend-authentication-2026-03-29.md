# Security Audit — Finance.Authentication

**Date**: 2026-03-29
**Scope**: `Finance.Authentication/**` (`Authorization/`, `Services/`, `Options/`, `Extensions/`)
**Reviewer**: GitHub Copilot

---

## Findings

### 1. `services.BuildServiceProvider()` Called at DI Registration Time — Duplicate Root Container

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Authentication/Extensions/AuthenticationExtensions.cs](FinanceBackEnd/src/Finance.Authentication/Extensions/AuthenticationExtensions.cs#L48) |
| **Lines** | [L48–L51](FinanceBackEnd/src/Finance.Authentication/Extensions/AuthenticationExtensions.cs#L48) |
| **Description** | `services.BuildServiceProvider()` is called inside the `ConfigureAuth0Authentication` extension, producing a second root-level `IServiceProvider`. This is an ASP.NET Core anti-pattern with several consequences: (1) The captured `sp` only sees services registered *before* this call — anything registered after will be absent in all authorization policy closures. (2) Singleton services become duplicated: the application has two separate singleton instances (one in the real root, one in the hand-built root). (3) `sp.GetRequiredService<FinanceDbContext>()` on line 49 resolves a scoped `DbContext` from a root provider, which is never disposed — a concrete `FinanceDbContext` connection is leaked for the lifetime of the application. ASP.NET Core itself emits a `DI4003` warning for this pattern. |
| **Priority** | High |
| **Recommendation** | Remove the `BuildServiceProvider()` call. Use `IServiceProvider` from the request pipeline instead: inject `IServiceScopeFactory` or defer the per-request provider lookup to the authorization assertion lambda, which already receives an `HttpContext` from which `IServiceProvider` can be resolved via `context.Resource`. Register policies by name with `AddAuthorization` and implement each as a proper `IAuthorizationHandler` receiving scoped dependencies via constructor injection. |

---

### 2. Authorization Policy Assertions Execute Synchronous Blocking DB Queries on Every Request

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Authentication/Authorization/Base/RolePolicy.cs](FinanceBackEnd/src/Finance.Authentication/Authorization/Base/RolePolicy.cs#L24) |
| **Lines** | [L24–L32](FinanceBackEnd/src/Finance.Authentication/Authorization/Base/RolePolicy.cs#L24) |
| **Description** | `RolePolicy.AssertionAction` executes `.FirstOrDefault()` — a synchronous, blocking EF Core call — inside an `AuthorizationHandlerContext` assertion that fires for every protected request. ASP.NET Core's authorization pipeline is async, but the `policy.RequireAssertion(Func<AuthorizationHandlerContext, bool>)` overload (not `RequireAssertion(Func<AuthorizationHandlerContext, Task<bool>>)`) is used. Synchronous DB calls on thread-pool threads block them for the full DB round-trip. Under concurrent load this causes thread pool exhaustion, degrading latency and eventually availability for all endpoints — including non-financial endpoints that share the same policy. |
| **Priority** | High |
| **Recommendation** | Replace `RequireAssertion(context => … )` with the async variant `RequireAssertion(async context => … )` and use `await dbContext.User.FirstOrDefaultAsync(…)` throughout. Better still, migrate each policy to a dedicated `AuthorizationHandler<TRequirement>` implementing `HandleRequirementAsync`, which is the idiomatic ASP.NET Core pattern and naturally enables async DB access. |

---

### 3. `AdminOrOwnerPolicy` Issues Two Separate DB Round-Trips Per Authorization Check

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Authentication/Authorization/Policies/AdminOrOwnerPolicy.cs](FinanceBackEnd/src/Finance.Authentication/Authorization/Policies/AdminOrOwnerPolicy.cs#L22) |
| **Lines** | [L22–L38](FinanceBackEnd/src/Finance.Authentication/Authorization/Policies/AdminOrOwnerPolicy.cs#L22) |
| **Description** | `AdminOrOwnerPolicy.AssertionAction` executes two separate `Any()` queries against the database: first to check whether the user is an Admin, then a second to check whether they are an Owner. Both queries load the full `User` entity including related `Identities` and `Roles` collections. Because authorization runs on every request for every endpoint using this policy, these doubled round-trips compound the performance issue from finding 2. A user with the `Admin` role always short-circuits on the first query, but `Owner`-only users always incur both queries. |
| **Priority** | Medium |
| **Recommendation** | Combine into a single query: `.Any(u => u.Identities.Any(i => i.SourceId == userIdClaim) && u.Roles.Any(r => r.Id == RoleEnum.Admin || r.Id == RoleEnum.Owner))`. This reduces two DB round-trips to one and returns the same result. |

---

### 4. `Auth0UserValidationService` Instantiates `HttpClient` Directly — Socket Exhaustion Risk

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs](FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs#L117) |
| **Lines** | [L117](FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs#L117) |
| **Description** | `GetManagementApiTokenAsync` constructs `new HttpClient()` on every call. `HttpClient` instances hold open TCP connections; instantiating them per-invocation bypasses connection pooling and exhausts ephemeral ports under any reasonable request volume. The client also inherits the default 100-second timeout with no explicit configuration. `Auth0UserValidationService` is registered as `Scoped` (`AuthenticationExtensions.cs` L57), so a new `HttpClient` is created on every call within a request. |
| **Priority** | High |
| **Recommendation** | Register a named or typed `HttpClient` via `services.AddHttpClient<Auth0UserValidationService>()` and inject `HttpClient` (or `IHttpClientFactory`) through the constructor. Configure a reasonable timeout (e.g. 10 seconds) and retry policy via Polly if needed. |

---

### 5. Management API Access Token Not Cached — Rate Limit Risk

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs](FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs#L95) |
| **Lines** | [L95–L144](FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs#L95) |
| **Description** | `GetManagementApiClientAsync` calls `GetManagementApiTokenAsync` on every invocation, issuing a fresh `POST /oauth/token` client_credentials request to Auth0 each time. Auth0 Management API tokens have a 24-hour TTL and are explicitly designed to be cached for reuse. Auth0 enforces strict per-tenant rate limits on the token endpoint (default: 2 requests/second, burst to 10). When `ValidateUserExistsAsync` is called repeatedly (e.g. during startup seeding or in rapid succession), this can exhaust the rate limit and cause all Management API calls to fail with `429 Too Many Requests`, silently returning `false` for all user validations. |
| **Priority** | Medium |
| **Recommendation** | Cache the `access_token` in a singleton `IMemoryCache` entry with an expiry of `expires_in - 60` seconds (a one-minute buffer before the token actually expires). On cache hit, skip the `/oauth/token` call entirely. This is the documented Auth0 best practice for machine-to-machine tokens. |

---

### 6. `ApplicationOptions.ClientSecret` Defined in `Auth0Options` But Unused — Dead Secret Field

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Authentication/Options/Auth0Options.cs](FinanceBackEnd/src/Finance.Authentication/Options/Auth0Options.cs#L45) |
| **Lines** | [L35–L45](FinanceBackEnd/src/Finance.Authentication/Options/Auth0Options.cs#L45) |
| **Description** | `ApplicationOptions.ClientSecret` is declared in the options model and exposed as a placeholder in `appsettings.json` ([L17](FinanceBackEnd/src/Finance.Api/appsettings.json#L17)), but it is never read anywhere in the codebase. The presence of an unused `ClientSecret` field in source-tracked configuration creates a social-engineering risk: developers may assume this field needs to be populated and commit a real secret to `appsettings.json`, which is source-controlled. Reviewers inspecting `appsettings.json` have no way to distinguish populated-and-real from populated-and-placeholder without auditing usages. |
| **Priority** | Medium |
| **Recommendation** | Remove `ClientSecret` from `ApplicationOptions` and its corresponding entry in `appsettings.json` if it genuinely has no use. If it is reserved for future use, add a comment in `appsettings.json` explicitly marking it as unused, and add a `[JsonIgnore]` / no-serialization guard so it cannot be accidentally logged. Secrets that are in use (`ManagementApi.ClientSecret`) should be supplied exclusively via environment variables or user secrets, never as default values in committed `appsettings.json`. |

---

### 7. `ValidateUserExistsAsync` Returns `false` on Configuration Failure — Silent Misconfiguration Masking

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs](FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs#L29) |
| **Lines** | [L29–L33](FinanceBackEnd/src/Finance.Authentication/Services/Auth0UserValidationService.cs#L29) |
| **Description** | When the Auth0 Management API is not configured (`Domain`, `ClientId`, or `ClientSecret` missing), `ValidateUserExistsAsync` logs a warning and returns `false`. Callers receive the same return value as "user does not exist in Auth0." Currently this is only called from `DatabaseSeeder`, which treats `false` as a reason to skip seeding — acceptable for startup logic. However, if this service is extended to guard authorization decisions (a plausible future path given its interface), a misconfigured environment would silently block all users rather than raising an error, making the failure mode invisible without log inspection. |
| **Priority** | Medium |
| **Recommendation** | Introduce a distinct result type or exception to differentiate "user not found" from "service unavailable / misconfigured." A simple approach is a `Result<bool>` or a custom `AuthValidationResult` with a `ServiceUnavailable` state. For startup-time usage, throw `InvalidOperationException` early if the configuration is incomplete, which surfaces the misconfiguration immediately rather than silently skipping operations. |
