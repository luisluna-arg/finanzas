# Security Audit — Finance.Application Commands

**Date**: 2026-03-29
**Scope**: `Finance.Application/Commands/**`, `Finance.Api/Controllers/**`, `Finance.Persistence/FinanceDbContext.cs`, `Finance.Api/Core/Config/ConfigExtensions.cs`
**Reviewer**: GitHub Copilot

---

## Findings

### 1. BOLA — `userId` URL Parameter Silently Ignored in Resource Endpoints

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Controllers/Commands/ResourceCommandController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Commands/ResourceCommandController.cs#L17) |
| **Lines** | [L17–L37 (command)](FinanceBackEnd/src/Finance.Api/Controllers/Commands/ResourceCommandController.cs#L17), [L17 (query)](FinanceBackEnd/src/Finance.Api/Controllers/Queries/ResourceQueryController.cs#L17) |
| **Description** | `SetFundOwner(Guid fundId, Guid userId)` and `DeleteFundOwner(Guid fundId, Guid userId)` accept `userId` from the URL but never use it — only `fundId` is forwarded to `fundService.SetOwner(fundId)` and `fundService.DeleteOwner(fundId)`. The same is true in `ResourceQueryController.GetFundOwner`. The operation is always performed using the token's authenticated identity, making the `userId` URL segment a misleading no-op. A caller believing they are assigning ownership *to* a specific user ID is silently wrong. |
| **Priority** | High |
| **Recommendation** | Either remove the `userId` route segment and document that ownership always binds to the caller, or validate the supplied `userId` against the token claim and pass it through to the service. Do not accept parameters from the request that are never used in authorization or business logic. |

---

### 2. Financial Entity Data Serialized to Console in Catch Blocks

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs](FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs#L43) |
| **Lines** | [L43](FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs#L43), [L68](FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs#L68), [L88](FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs#L88), [L111](FinanceBackEnd/src/Finance.Api/Controllers/Base/ApiBaseQueryController.cs#L111) |
| **Description** | Multiple `catch` blocks call `Console.WriteLine($"Entity: {System.Text.Json.JsonSerializer.Serialize(entity)}")`, writing a full JSON serialization of the domain entity to stdout. Entities returned by query handlers include `Money`-typed fields (balances, amounts, totals). In a Docker deployment stdout is shipped to a centralized log sink (Grafana/Loki per the local infra). This exposes full financial records in logs on any mapping failure. |
| **Priority** | High |
| **Recommendation** | Remove the entity serialization lines entirely from all catch blocks. Log only the exception type and a sanitized message (e.g. entity type name and ID). Never serialize a domain entity to a log sink. If mapping diagnostics are needed during development, gate them behind a debug flag or use a log scrubber projection. |

---

### 3. No Rate Limiting on Financial Write Endpoints

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs](FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs#L26) |
| **Lines** | L26–L56 (entire `MainServices` registration), L117–L135 (middleware pipeline) |
| **Description** | There is no rate limiting middleware registered anywhere in the application. Financial write endpoints — credit card payments (`POST /api/credit-card-payments`), income creation (`POST /api/incomes`), movement uploads (`POST /api/movements/upload`), exchange rate creation (`POST /api/currencies/exchange-rates`) — are unrestricted. An authenticated attacker can issue an unlimited number of writes or bulk file uploads in parallel. There are also no idempotency keys on write operations. |
| **Priority** | High |
| **Recommendation** | Add the built-in .NET 8 rate limiter (`services.AddRateLimiter`) with a fixed or sliding-window policy scoped per authenticated user. Apply a stricter policy to bulk upload endpoints and financial write operations. Also consider idempotency keys for payment and transfer endpoints to prevent duplicate submissions. |

---

### 4. `EnumHelper.Parse` Silently Returns Default on Invalid Input

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Helpers/EnumHelper.cs](FinanceBackEnd/src/Finance.Application/Helpers/EnumHelper.cs#L16) |
| **Lines** | [L16](FinanceBackEnd/src/Finance.Application/Helpers/EnumHelper.cs#L16) |
| **Description** | `EnumHelper.Parse<TEnum>` uses `Enum.TryParse` and returns `default(TEnum)` for any unrecognized input string instead of rejecting it. For `DateTimeKind` this means an invalid value silently resolves to `DateTimeKind.Unspecified` (0). This parameter controls how timestamps are interpreted in bulk financial file uploads (`dateKind` query string on `POST /api/movements/upload`, `POST /api/debits/*/upload`, `POST /api/iol-investment/upload`, `POST /api/credit-card-transactions/upload`). A missing or misspelled `dateKind` causes all dates in an uploaded file to be stored with the wrong timezone offset without any error surfacing to the caller. |
| **Priority** | Medium |
| **Recommendation** | Throw `ArgumentException` (or return a `BadRequest` result) when `Enum.TryParse` fails. In the controllers, validate `dateKind` before dispatching the command and return HTTP 400 for unrecognized values. |

---

### 5. Financial Amounts Stored as Plain Decimal — No Encryption at Rest

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs](FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs#L6) |
| **Lines** | [L6–L17](FinanceBackEnd/src/Finance.Persistence/TypeConverters/MoneyValueConverter.cs#L6) |
| **Description** | `MoneyValueConverter` maps `Money` → `decimal` with no encryption. All monetary columns across every financial entity (movements, debits, incomes, credit card transactions, payments, exchange rates, currency conversions) are stored as plain `numeric` values in PostgreSQL. Anyone with direct database access (via a compromised connection string, a misconfigured Postgres ACL, or a backup leak) can read every user's financial data in cleartext. |
| **Priority** | Medium |
| **Recommendation** | Apply symmetric column-level encryption (e.g. AES-256) for all monetary and personally sensitive columns using an EF value converter that encrypts on write and decrypts on read. Manage the encryption key via a secret store (Azure Key Vault, HashiCorp Vault, or at minimum Docker secrets), never in `appsettings.json`. Alternatively, evaluate PostgreSQL Transparent Data Encryption or pgcrypto if application-layer encryption is not feasible. |

---

### 6. Wildcard CORS Policy on Financial API

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs](FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs#L52) |
| **Lines** | [L52](FinanceBackEnd/src/Finance.Api/Core/Config/ConfigExtensions.cs#L52) |
| **Description** | The CORS policy is configured as `AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()`. Any web page on any domain can issue credentialed-looking requests to this API. This makes cross-origin request forgery attacks straightforward and removes the browser's same-origin protection for any frontend that interacts with this API. |
| **Priority** | Medium |
| **Recommendation** | Replace the wildcard with an explicit allow-list loaded from configuration (e.g. `appsettings.json` under `Cors:AllowedOrigins`). In development the list can include `localhost:5100` / `localhost:5200`; in production only the deployed frontend origins. |

---

### 7. Unhandled `Exception` Throws in Services Propagate as HTTP 500

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Application/Services/UserService.cs](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L37) |
| **Lines** | [L37](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L37), [L50](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L50), [L59](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L59), [L85](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L85), [L99](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L99), [L108](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L108), [L134](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L134), [L145](FinanceBackEnd/src/Finance.Application/Services/UserService.cs#L145) |
| **Description** | Business logic errors (duplicate source IDs, not-found entities, failed sub-operations) are expressed as `throw new Exception("message")`. Because there is no global exception handling middleware, these propagate as unhandled HTTP 500 responses. In development mode ASP.NET Core may include a stack trace in the response body. Internal error messages (e.g. `"User with the same source IDs already exists"`) reveal implementation details to the caller. |
| **Priority** | Medium |
| **Recommendation** | Register a global exception handler (`app.UseExceptionHandler`) that catches unhandled exceptions and returns a generic `ProblemDetails` response (RFC 7807) with no internal message or stack trace. Introduce domain-specific exception types (e.g. `DomainNotFoundException`, `DuplicateEntityException`) to allow the handler to map them to appropriate HTTP status codes (404, 409) without leaking internals. |

---

### 8. `SessionController` Exposes Full JWT Claim Dictionary

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Api/Controllers/SessionController.cs](FinanceBackEnd/src/Finance.Api/Controllers/SessionController.cs#L39) |
| **Lines** | [L39–L46](FinanceBackEnd/src/Finance.Api/Controllers/SessionController.cs#L39) |
| **Description** | `GET /api/session/me` serializes the entire `HttpContext.User.Claims` collection — grouped by type — into the response. This includes internal Auth0 claims (`iss`, `aud`, `iat`, `exp`, internal permission scopes, and any custom namespace claims) beyond what the frontend needs. The endpoint is protected by the weaker `AuthenticatedPolicy` (any authenticated user) rather than `AdminOrOwnerPolicy`, so any valid token holder can retrieve this data. |
| **Priority** | Medium |
| **Recommendation** | Remove the `Claims` field from the response. Expose only the specific fields the frontend requires (user ID, display name, roles). If claim inspection is needed for debugging, restrict the claim dump to an admin-only endpoint or a development-only response path. |
