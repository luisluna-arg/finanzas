# Security Audit — FinanceApp Data Layer
**Scope**: `FinanceFrontEnd/FinanceApp/app/data/`
**Date**: 2026-03-29
**Reviewer**: GitHub Copilot

---

## Summary

3 issues found across 4 files. The most critical risk is a module-level HTTPS agent that disables TLS certificate validation in all environments, exposing all backend API traffic — including access tokens and financial data — to interception. A closely related high-severity issue is that Axios error objects (which carry the full request config, including the `Authorization` header) are passed directly to an always-on production logger with no redaction.

---

## Findings

---

### R1 — TLS certificate validation disabled globally

| Field | Detail |
|---|---|
| **Component / File** | [app/data/BackendClient.ts](FinanceFrontEnd/FinanceApp/app/data/BackendClient.ts#L18) |
| **Line** | L18 |
| **Impact** | `new Agent({ rejectUnauthorized: false })` is instantiated as a module-level singleton and passed to every query class. This disables TLS certificate verification for all server-to-backend HTTPS calls in **every environment, including production**. An attacker with network access between the SSR server and the backend API can present any certificate and intercept all traffic — including Bearer access tokens and the full payloads of every financial API response. |
| **Priority** | Critical |
| **Recommendation** | Remove `rejectUnauthorized: false`. TLS validation must be enabled in production. If self-signed certificates are required in local development only, conditionally construct the agent based on `NODE_ENV`: |

```ts
// BackendClient.ts
const httpsAgent = new Agent(
  process.env.NODE_ENV !== 'production'
    ? { rejectUnauthorized: false }   // dev only
    : {}                               // full validation in production
);
```

If a private CA is in use in production, supply `ca` with the CA certificate bundle rather than disabling validation entirely.

---

### R2 — Bearer token exposed in production error logs via unredacted Axios errors

| Field | Detail |
|---|---|
| **Component / File** | [app/data/base/BaseQuery.ts](FinanceFrontEnd/FinanceApp/app/data/base/BaseQuery.ts#L41), [app/data/base/BasePaginatedQuery.ts](FinanceFrontEnd/FinanceApp/app/data/base/BasePaginatedQuery.ts#L36), [app/data/queries/CreditCardStatementQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CreditCardStatementQuery.ts#L21), [app/utils/logger.server.ts](FinanceFrontEnd/FinanceApp/app/utils/logger.server.ts#L19) |
| **Line** | `BaseQuery.ts` L41–43, `BasePaginatedQuery.ts` L35–37, `CreditCardStatementQuery.ts` L20–22 |
| **Impact** | All three error handlers call `serverLogger.error('Error:', error)` passing the raw Axios `AxiosError` object. This object includes `error.config.headers.Authorization: "Bearer <token>"`. The `serverLogger.error` method (in `logger.server.ts`) has **no production guard** — it always calls `console.error`. The logger performs **no redaction**. Any API failure therefore writes the current user's access token into the process output/log stream in production. Log aggregators (stdout, container logs, Datadog, etc.) will capture and persist these tokens for their full retention period. |
| **Priority** | High |
| **Recommendation** | Strip sensitive fields before logging. The minimum safe change is to log only the error message and status, not the full error object: |

```ts
// BaseQuery.ts — in the catch blocks
} catch (error) {
  const msg = error instanceof Error ? error.message : String(error);
  const status = (error as { response?: { status?: number } })?.response?.status;
  serverLogger.error('BaseQuery.get failed:', { endpoint: String(this.getEndpoint), status, message: msg });
  throw error;
}
```

Apply the same pattern in `BasePaginatedQuery` and `CreditCardStatementQuery`. Do not log `error.config` or pass the full `AxiosError` to any log sink.

Additionally, note that `CurrenciesQuery.getById` (line 16–18) has no `try/catch` at all — an Axios failure there propagates the raw error up through the loader without any log scrubbing, which means the full error (including the `config` object) may eventually be caught and logged by a less controlled boundary.

---

### R3 — Internal backend endpoint URLs logged in production

| Field | Detail |
|---|---|
| **Component / File** | [app/data/base/BaseQuery.ts](FinanceFrontEnd/FinanceApp/app/data/base/BaseQuery.ts#L42) |
| **Line** | L42 |
| **Impact** | `serverLogger.error('this.getEndpoint:', this.getEndpoint)` is called unconditionally on every request failure and always runs in production (same `serverLogger.error` without environment guard). This writes the raw internal backend URL (e.g. `http://backend:5000/api/finances/credit-cards`) to production logs on every error. Internal route structure is useful reconnaissance for attackers who gain read access to logs. |
| **Priority** | Medium |
| **Recommendation** | Remove this log line. The endpoint is already captured implicitly in the structured log entry recommended in R2. If endpoint logging is required, include it as a structured field that can be filtered rather than a standalone log line. |

---

## Files Reviewed

| File | Status |
|---|---|
| [app/data/BackendClient.ts](FinanceFrontEnd/FinanceApp/app/data/BackendClient.ts) | Issue found (R1) |
| [app/data/getBackendClient.ts](FinanceFrontEnd/FinanceApp/app/data/getBackendClient.ts) | No issues |
| [app/data/base/axiosConfig.ts](FinanceFrontEnd/FinanceApp/app/data/base/axiosConfig.ts) | No issues |
| [app/data/base/BaseQuery.ts](FinanceFrontEnd/FinanceApp/app/data/base/BaseQuery.ts) | Issues found (R2, R3) |
| [app/data/base/BasePaginatedQuery.ts](FinanceFrontEnd/FinanceApp/app/data/base/BasePaginatedQuery.ts) | Issue found (R2) |
| [app/data/queries/BanksQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/BanksQuery.ts) | No issues |
| [app/data/queries/CatalogBanksQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CatalogBanksQuery.ts) | No issues |
| [app/data/queries/CatalogCurrenciesQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CatalogCurrenciesQuery.ts) | No issues |
| [app/data/queries/CatalogFrequenciesQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CatalogFrequenciesQuery.ts) | No issues |
| [app/data/queries/CreditCardPaymentQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CreditCardPaymentQuery.ts) | Empty (not in use) |
| [app/data/queries/CreditCardQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CreditCardQuery.ts) | No issues |
| [app/data/queries/CreditCardStatementQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CreditCardStatementQuery.ts) | Issue found (R2) |
| [app/data/queries/CreditCardTransactionQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CreditCardTransactionQuery.ts) | Empty (not in use) |
| [app/data/queries/CurrenciesQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CurrenciesQuery.ts) | Issue found (R2 — no error handling in `getById`) |
| [app/data/queries/CurrencyExchangeRatesQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/CurrencyExchangeRatesQuery.ts) | No issues |
| [app/data/queries/DebitsQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/DebitsQuery.ts) | No issues |
| [app/data/queries/FrequenciesQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/FrequenciesQuery.ts) | No issues |
| [app/data/queries/PaginatedDebitsQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/PaginatedDebitsQuery.ts) | No issues |
| [app/data/queries/PaginatedSubscriptionsQuery.ts](FinanceFrontEnd/FinanceApp/app/data/queries/PaginatedSubscriptionsQuery.ts) | No issues |
