# Backend Security Audit — 2026-03-29

**Scope**: Controller layer only (`Controllers/**/*.cs`, base controllers, middleware pipeline in `Program.cs` / `ConfigExtensions.cs`)

---

## Findings

### 1. Financial Entity Data Serialized to Console in Catch Blocks

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs#L38) |
| **Line** | Lines 38–40, 57–59, 68–70, 87–90 |
| **Description** | Multiple `catch` blocks call `Console.WriteLine($"Entity: {System.Text.Json.JsonSerializer.Serialize(entity)}")`, serializing the full domain entity — which may contain balances, amounts, or other financial data — to the process stdout. In Docker/cloud deployments, stdout is typically shipped to a centralized log sink. |
| **Priority** | High |
| **Recommendation** | Remove the entity serialization lines from the catch blocks entirely. Log only the exception type and message, never the entity payload. If diagnostic information is needed, use structured logging with a scrubbed projection (e.g. entity ID only). |

---

### 2. Wildcard CORS Policy on Financial API

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs](FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs#L52) |
| **Line** | `policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()` |
| **Description** | The sole CORS policy applied to all routes (`AllowOriginsForCORSPolicy`) permits requests from any origin, with any headers and any HTTP method. For a financial API secured by JWT, this eliminates the browser's same-origin protection and allows arbitrary websites to make credentialed-looking cross-origin requests — widening the attack surface significantly. |
| **Priority** | High |
| **Recommendation** | Replace `AllowAnyOrigin()` with an explicit allowlist of trusted frontend origins (e.g. `WithOrigins("https://app.example.com")`). Configure this list via environment variable/appsettings so it can be tightened per environment. |

---

### 3. `SetRoles` Endpoint Missing Admin-Only Authorization

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Controllers/Commands/UserCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/UserCommandController.cs#L64) |
| **Line** | 64–70 |
| **Description** | The `PUT /api/users/{userId}/roles` endpoint inherits `[Authorize(Policy = "AdminOrOwnerPolicy")]` from the base class but does not add an `[Authorize(Roles = "Admin")]` restriction. An authenticated user satisfying the "Owner" role can supply any `userId` in the URL path, have it assigned directly to `command.UserId`, and call this endpoint to modify roles for any user account in the system — including elevating themselves to Admin. |
| **Priority** | Critical |
| **Recommendation** | Add `[Authorize(Roles = "Admin")]` to the `SetRoles` action. Additionally, validate inside the handler that the requesting user's token `sub` claim matches either the target user or an admin role claim, so authorization cannot be bypassed even if the policy is accidentally relaxed. |

---

### 4. No Global Exception Handler Middleware

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs](FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs#L112) |
| **Line** | `MainConfiguration` method — no `UseExceptionHandler` call |
| **Description** | The middleware pipeline has no `app.UseExceptionHandler(...)` registration. Unhandled exceptions that propagate out of controllers will be handled by ASP.NET Core's default behavior, which returns a 500 with no filtering. Without an explicit handler, there is no guaranteed boundary preventing internal exception messages or partial stack frames from reaching the response body in edge cases (e.g. negotiate failures, middleware exceptions). |
| **Priority** | Medium |
| **Recommendation** | Register a global exception handler before `UseRouting()`. At minimum: `app.UseExceptionHandler("/error")` with a dedicated error endpoint that returns a generic `ProblemDetails` response regardless of environment. For development, the existing `AddDatabaseDeveloperPageExceptionFilter` is registered but its companion `UseDeveloperExceptionPage` is never called. |

---

### 5. Internal Error Messages Returned to Clients via `BadRequest`

| Field | Detail |
|---|---|
| **File** | Multiple — [CommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Base/CommandController.cs#L59), [UserCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/UserCommandController.cs#L31), [IdentityCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/IdentityCommandController.cs#L31) |
| **Line** | Varies per controller |
| **Description** | `BadRequest(result.ErrorMessage)` throughout command controllers returns raw service-layer error strings to the API consumer. Depending on the message content (e.g. "App Module not found, Id: {guid}", EF constraint messages), this can disclose internal identifiers, schema details, or business-logic specifics useful for probing the system. |
| **Priority** | Medium |
| **Recommendation** | Map service error messages to a generic `ProblemDetails` DTO at the controller boundary. Use error codes rather than free-text messages in responses. Reserve detailed messages for structured server-side logs only. |

---

### 6. No Rate Limiting on Financial Write Endpoints

| Field | Detail |
|---|---|
| **File** | [Program.cs](FinanceBackEnd/src/Finance.Api/Program.cs) / [ConfigExtensions.cs](FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs#L112) |
| **Line** | `MainServices` and `MainConfiguration` — no rate limiter registered or applied |
| **Description** | There is no rate limiting or idempotency control anywhere in the middleware pipeline or on individual controllers. Financial write endpoints — including `POST /api/movements`, `POST /api/credit-card-payments`, `POST /api/incomes`, `POST /api/funds`, `POST /api/currencies/conversions` — can be called at unlimited frequency by any authenticated user. This enables replay attacks, accidental double-submission, and amplification of any business-logic bugs. |
| **Priority** | High |
| **Recommendation** | Register `AddRateLimiter` (ASP.NET Core 7+ built-in) with a fixed-window or token-bucket policy. Apply a stricter per-user limiter to all `POST`/`PUT`/`DELETE` routes under `/api/`. Consider idempotency keys for payment-specific endpoints. |

---

### 7. Fund Ownership Disclosure without Caller Validation (BOLA)

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Controllers/Queries/ResourceQueryController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Queries/ResourceQueryController.cs#L17) |
| **Line** | 17–25 |
| **Description** | `GET /api/resources/fund/{fundId}/owner/{userId}` accepts a `userId` in the URL but does not use it in the query — `GetFundOwnershipQuery` is called with `fundId` only. Any authenticated user can supply any `fundId` and retrieve ownership data for funds they do not own. The stale `userId` path segment creates the appearance of per-user scoping while providing none. |
| **Priority** | Medium |
| **Recommendation** | Either enforce that the authenticated user's `sub` matches the `userId` path segment before executing the query, or remove the `userId` segment entirely and scope the query to the caller's identity at the handler level. |

---

### 8. File Upload Endpoints — No Controller-Level Content-Type or Extension Validation

| Field | Detail |
|---|---|
| **File** | [MovementCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/MovementCommandController.cs#L37), [CreditCardTransactionCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/CreditCardTransactionCommandController.cs) (line ~31), [IOLInvestmentCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/IOLInvestmentCommandController.cs#L37), [DebitCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/DebitCommandController.cs#L37) |
| **Line** | Each `Upload` action |
| **Description** | All `IFormFile` upload endpoints pass the file directly to the dispatcher without any controller-level validation of `ContentType`, file extension, or size. An attacker with a valid token can upload arbitrarily large files or files with misleading MIME types, potentially exploiting downstream Excel parsing logic. |
| **Priority** | Medium |
| **Recommendation** | At the controller boundary, validate `file.ContentType` against an allowlist (e.g. `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`) and `file.FileName` extension. Enforce a maximum file size limit (also configure `MaxRequestBodySize` in the host). |

---

### 9. Unresolved TODO: Missing Authorization Policy on Summary Endpoints

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Controllers/Queries/SummaryQueryController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Queries/SummaryQueryController.cs#L17) |
| **Line** | 17 |
| **Description** | A `// TODO I should be able to set Owner policy here, check authorization policies` comment sits above the controller class. The controller inherits `SecuredApiController` (which carries `[Authorize(Policy = "AdminOrOwnerPolicy")]`), so requests are not unauthenticated — but the "Owner" side of that policy and whether these aggregate financial summary queries are correctly scoped to the caller's data is explicitly unresolved by the team. |
| **Priority** | Medium |
| **Recommendation** | Resolve the TODO: audit `GetCurrentFundsQuery`, `GetTotalExpensesQuery`, `GetCurrentIncomesQuery`, `GetCurrentInvestmentsQuery`, and `GetGeneralSummaryQuery` to confirm each applies ownership filtering against the authenticated user's identity. Remove the TODO comment once verified or the policy has been tightened. |

---

## Summary Table

| # | Priority | File | Issue |
|---|---|---|---|
| 1 | High | `ApiBaseQueryController.cs` | Financial entity data serialized in catch blocks |
| 2 | High | `ConfigExtensions.cs` | Wildcard CORS (`AllowAnyOrigin`) |
| 3 | Critical | `UserCommandController.cs` | `SetRoles` endpoint lacks Admin-only authorization |
| 4 | Medium | `ConfigExtensions.cs` | No global exception handler middleware |
| 5 | Medium | Multiple command controllers | Internal error messages returned to clients |
| 6 | High | `Program.cs` / `ConfigExtensions.cs` | No rate limiting on financial write endpoints |
| 7 | Medium | `ResourceQueryController.cs` | Fund ownership disclosure — BOLA |
| 8 | Medium | Multiple upload controllers | No content-type / extension validation on file uploads |
| 9 | Medium | `SummaryQueryController.cs` | Unresolved TODO on authorization policy |
