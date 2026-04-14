# Security Audit — FinanceApp Remaining Modules
**Scope**: `FinanceFrontEnd/FinanceApp/app/` — `hooks/`, `lib/`, `middleware/`, `telemetry/`, `types/`, `utils/`, `entry.client.tsx`
**Date**: 2026-03-29
**Reviewer**: GitHub Copilot

---

## Summary

3 issues found. The remaining modules are largely infrastructure (logging utilities, OTel tracing setup, URL construction helpers, type definitions). The most actionable finding is a structural weakness in `SafeLogger`'s redaction logic that leaves nested sensitive keys unprotected in production error logs. The middleware cookie-logging concern is carried forward from the routes audit as the originating file is in this scope.

---

## Findings

---

### R1 — SafeLogger.redact() does not recurse into nested objects

| Field | Detail |
|---|---|
| **Component / File** | [app/utils/SafeLogger.ts](FinanceFrontEnd/FinanceApp/app/utils/SafeLogger.ts#L8) |
| **Lines** | L8–34 (redact function), L50 (error — always-on path) |
| **Impact** | The `redact()` function iterates only the top-level keys of the passed object. A nested object whose inner keys contain `token`, `secret`, `password`, or `access` is not redacted — only the outer container key is checked. For example, `SafeLogger.error('Err', { response: { accessToken: 'xxx' } })` leaves `accessToken` fully visible in production logs since the key `response` does not match the filter. `SafeLogger.error` always executes (no `isDev` guard), making this the only redaction path that matters in production. Callers in `session.server.ts` and `tokenRefresh.server.ts` pass caught exceptions and raw object trees that may contain nested token values. |
| **Priority** | High |
| **Recommendation** | Make `redact()` recursive: |

```ts
function redact(obj: unknown, depth = 0): unknown {
  if (depth > 5 || !obj || typeof obj !== 'object') return obj;
  const clone: Record<string, unknown> = Array.isArray(obj) ? [] as unknown as Record<string, unknown> : {};
  for (const k of Object.keys(obj as Record<string, unknown>)) {
    const lower = k.toLowerCase();
    if (
      lower.includes('secret') || lower.includes('token') ||
      lower.includes('password') || lower.includes('access')
    ) {
      clone[k] = '[REDACTED]';
    } else {
      clone[k] = redact((obj as Record<string, unknown>)[k], depth + 1);
    }
  }
  return clone;
}
```

---

### R2 — Cookie header logged unconditionally in production via Pino middleware

| Field | Detail |
|---|---|
| **Component / File** | [app/middleware/logger.server.ts](FinanceFrontEnd/FinanceApp/app/middleware/logger.server.ts#L22) |
| **Lines** | L22–34 (`createRequestLogger`) |
| **Impact** | Every inbound request is logged by the always-on Pino logger with `cookies: cookies.substring(0, 100)` and `setCookie: setCookie.substring(0, 100)`. The `Cookie` header contains the `__user_session` value — a server session UUID unique to each user session. While not a raw access token, the UUID is a sufficient credential for session replay: any actor with log access can replay the cookie to authenticate as that user. The 100-character truncation does not meaningfully limit this since the UUID is 36 characters. This was originally identified in the routes audit (R5) but the source is this middleware file. |
| **Priority** | Medium |
| **Recommendation** | Remove `cookies` and `setCookie` from the log payload entirely. If cookie *presence* (not value) is needed for debugging, log a boolean: |

```ts
logger.info(
  {
    method: request.method,
    path: url.pathname,
    status: statusCode,
    hasSession: !!request.headers.get('cookie'),
    userAgent: request.headers.get('user-agent')?.substring(0, 50),
  },
  `${request.method} ${url.pathname} ${statusCode}`
);
```

---

### R3 — OTel server instrumentation auto-captures spans for Auth0 HTTP calls without explicit sanitization

| Field | Detail |
|---|---|
| **Component / File** | [app/telemetry/tracing.server.ts](FinanceFrontEnd/FinanceApp/app/telemetry/tracing.server.ts#L26) |
| **Lines** | L26–36 (`HttpInstrumentation`, `UndiciInstrumentation`) |
| **Impact** | When `OTEL_ENABLED=true`, `HttpInstrumentation` and `UndiciInstrumentation` auto-instrument all outbound HTTP calls from the Node.js process, including calls to `https://{AUTH0_DOMAIN}/oauth/token` (token refresh, which POSTs `client_secret`) and `https://{AUTH0_DOMAIN}/userinfo`. OTel's default Node HTTP instrumentation does not capture request bodies or the `Authorization` header, but it does record span attributes including the full URL, method, and `http.url` — which exposes the Auth0 domain and endpoint paths. If the OTLP backend (`OTEL_EXPORTER_OTLP_ENDPOINT`) is misconfigured as externally reachable, these spans are externally readable. Additionally, future OTel SDK version changes or misconfigured attribute limits could expand what is captured. No explicit `ignoreOutgoingRequestHook` is configured to exclude Auth0 calls. |
| **Priority** | Medium |
| **Recommendation** | Add an `ignoreOutgoingRequestHook` to exclude Auth0 domain calls from tracing: |

```ts
new HttpInstrumentation({
  ignoreIncomingRequestHook: (req) => { /* existing */ },
  ignoreOutgoingRequestHook: (req) => {
    const host = req.hostname ?? '';
    return host.includes(process.env.AUTH0_DOMAIN ?? '__none__');
  },
}),
```

Apply the same exclusion to `UndiciInstrumentation`. Ensure `OTEL_EXPORTER_OTLP_ENDPOINT` resolves to an internal-only host in all environments.

---

## Non-Issues (confirmed)

| Topic | Verdict |
|---|---|
| `hooks/use-mobile.ts` | Reads only `window.innerWidth` and `matchMedia` — no sensitive data |
| `lib/utils.ts` | Pure `clsx`/`tailwind-merge` utility — no data handling |
| `utils/common.ts` | `Params()` uses `encodeURIComponent` on both key and value — URL-safe |
| `utils/BackendUrl.ts` | `toString()` always encodes the backend path via `encodeURIComponent`, preventing path injection into the proxy URL |
| `utils/urlTreatment.ts` | All param construction uses `URLSearchParams` with `.append()` — safe |
| `utils/dates.ts` | Date formatting only — no auth or financial data |
| `utils/currency.constants.ts` | Exposes ARS/USD UUIDs in client bundle; these are catalog identifiers, not auth or financial values |
| `utils/JsonResponse.ts` | Response construction only; `details` field in `JsonErrorResponse` should not include raw exceptions at call sites (not a concern in this file itself) |
| `entry.client.tsx` | OTel client tracing is guarded by `window.__OTEL?.enabled`; `FetchInstrumentation` only traces `/api/proxy` calls which carry no auth headers (BFF pattern) |
| `types/` | Pure TypeScript interfaces — no runtime exposure |
| `telemetry/tracing.client.ts` | Browser OTLP exporter URL is read from `window.__OTEL.httpEndpoint` (server-injected config); no tokens or financial data included in spans |
