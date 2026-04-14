# Security Audit — FinanceApp Routes
**Scope**: `FinanceFrontEnd/FinanceApp/app/routes/` and `FinanceFrontEnd/FinanceApp/app/_disabled_routes/`
**Date**: 2026-03-29
**Reviewer**: GitHub Copilot

---

## Summary

7 issues found across 6 files. The most critical risk is two dead-code session files with hardcoded cookie signing secrets that would allow session forgery if the files were ever imported. A production stack trace leak and an unrestricted backend proxy round out the high-severity findings.

---

## Findings

---

### R1 — Hardcoded cookie signing secrets

| Field | Detail |
|---|---|
| **Component / File** | [app/services/auth/auth-session.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/auth-session.server.ts#L10), [app/services/auth/cookie-session.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/cookie-session.server.ts#L7) |
| **Line** | `auth-session.server.ts` L10, `cookie-session.server.ts` L7 |
| **Impact** | Cookie signing secrets `"s3cr3t"` and `"your-secret-key"` are hardcoded literals. Knowing these values allows an attacker to forge signed session cookies and impersonate any user without valid credentials. Both files appear to be dead code (no active imports found), but their presence in the repository is dangerous: they may be accidentally imported during future development, or activated by a transitive remix-auth dependency. |
| **Priority** | Critical |
| **Recommendation** | Delete both files if they are confirmed dead code. If either is ever needed, migrate the `secrets` array to read from environment variables following the pattern established in `session.server.ts` (lines 21–34). |

---

### R2 — Stack trace rendered in client HTML in all environments

| Field | Detail |
|---|---|
| **Component / File** | [app/root.tsx](FinanceFrontEnd/FinanceApp/app/root.tsx#L170) |
| **Line** | L170–L181 |
| **Impact** | The `ErrorBoundary` component renders `errorStack` (the full server-side stack trace) directly into the HTML response in all environments, including production. End users in a browser can see internal file paths, module names, and code structure — information that assists targeted exploitation. |
| **Priority** | High |
| **Recommendation** | Gate the stack trace rendering behind a dev-only check. In production, render only a generic message and an opaque error reference. |

```tsx
// root.tsx — ErrorBoundary
{process.env.NODE_ENV !== 'production' && errorStack && (
  <pre ...>{errorStack}</pre>
)}
```

---

### R3 — Unrestricted path forwarding in the API proxy

| Field | Detail |
|---|---|
| **Component / File** | [app/routes/api.proxy.ts](FinanceFrontEnd/FinanceApp/app/routes/api.proxy.ts#L9) |
| **Line** | Loader L9–L47, Action L105–L143 |
| **Impact** | The `path` query parameter is accepted from the client and forwarded to the backend API without allowlist validation. Any authenticated user can supply an arbitrary path (e.g. `?path=/admin/users`, `?path=/internal/config`) and have the SSR server proxy the request on their behalf using the server's access token. The backend's own authorization must serve as the only gate. Defence-in-depth is violated and the frontend proxy amplifies the blast radius of any backend authorization gap. |
| **Priority** | High |
| **Recommendation** | Validate `apiPath` against a strict allowlist of permitted path prefixes before constructing the upstream URL. Reject any path that does not match an approved pattern. |

```ts
const ALLOWED_PATH_PREFIXES = ['/api/finances/', '/api/catalog/'];

if (!ALLOWED_PATH_PREFIXES.some((prefix) => apiPath.startsWith(prefix))) {
  throw JsonErrorResponse('Forbidden path', HttpStatusConstants.FORBIDDEN);
}
```

---

### R4 — Script injection risk in SSR OTEL config serialization

| Field | Detail |
|---|---|
| **Component / File** | [app/root.tsx](FinanceFrontEnd/FinanceApp/app/root.tsx#L84) |
| **Line** | L84–L86 |
| **Impact** | `window.__OTEL=${JSON.stringify(otel)}` is written into a `dangerouslySetInnerHTML` inline script block. `JSON.stringify` does not escape `</script>` sequences. If `OTEL_OTLP_HTTP_ENDPOINT` contains the string `</script><script>`, the injected text breaks out of the script tag and executes arbitrary code in every visitor's browser. This vector is controlled by the server environment, but is exploitable via misconfigured hosting platforms, secret injection attacks, or supply chain compromise of the environment. |
| **Priority** | Medium |
| **Recommendation** | Use an HTML-safe JSON serializer. A minimal inline fix is to replace `</` with `<\/` in the serialized output: |

```tsx
dangerouslySetInnerHTML={{
  __html: `window.__OTEL=${JSON.stringify(otel).replace(/</g, '\\u003c')}`,
}}
```

---

### R5 — Session cookie values written to production logs

| Field | Detail |
|---|---|
| **Component / File** | [app/entry.server.tsx](FinanceFrontEnd/FinanceApp/app/entry.server.tsx#L40) |
| **Line** | L40–L52 |
| **Impact** | The always-on Pino logger records `cookiePreview: cookies?.substring(0, 50)` and `setCookiePreview: setCookie?.substring(0, 100)` on every request. The `__user_session` cookie signed value may appear in the first 50 characters of the `Cookie` header. Partial cookie values in log streams can assist session fixation attacks and violate data minimization requirements. |
| **Priority** | Medium |
| **Recommendation** | Remove `cookiePreview` and `setCookiePreview` from the Pino log record. The boolean flags `hasCookies` and `hasSetCookie` already convey the diagnostically useful information without exposing values. |

---

### R6 — No Content Security Policy configured

| Field | Detail |
|---|---|
| **Component / File** | [vite.config.ts](FinanceFrontEnd/FinanceApp/vite.config.ts), [app/entry.server.tsx](FinanceFrontEnd/FinanceApp/app/entry.server.tsx), [app/root.tsx](FinanceFrontEnd/FinanceApp/app/root.tsx) |
| **Line** | N/A |
| **Impact** | No `Content-Security-Policy` response header or meta tag is set anywhere in the application. Without a CSP, a successful XSS attack can exfiltrate all page data, inject scripts from arbitrary origins, and operate with no browser-enforced restrictions. This elevates the impact of every other XSS-class finding. |
| **Priority** | Medium |
| **Recommendation** | Add a `Content-Security-Policy` header in `entry.server.tsx` before the response is sent. Minimum viable policy: |

```ts
responseHeaders.set(
  'Content-Security-Policy',
  [
    "default-src 'self'",
    "script-src 'self' 'unsafe-inline'",   // tighten to nonce-based when feasible
    `connect-src 'self' https://${AUTH0_DOMAIN} ${OTEL_ENDPOINT}`,
    "img-src 'self' data: https:",
    "style-src 'self' 'unsafe-inline'",
    "frame-ancestors 'none'",
  ].join('; ')
);
```

The two `dangerouslySetInnerHTML` script tags in `root.tsx` (theme init and `window.__OTEL`) are what currently require `unsafe-inline`. Migrating them to nonce-based injection would allow that directive to be dropped.

---

### R7 — Missing auth guard in disabled route `debits.annual.tsx`

| Field | Detail |
|---|---|
| **Component / File** | [app/_disabled_routes/debits.annual.tsx](FinanceFrontEnd/FinanceApp/app/_disabled_routes/debits.annual.tsx#L4) |
| **Line** | L4–L17 |
| **Impact** | The loader returns internal API endpoint URL strings and hardcoded backend module UUIDs with no `requireAuth` call. If this file is moved back into `app/routes/` without modification it will immediately serve internal infrastructure details to unauthenticated clients. |
| **Priority** | Low |
| **Recommendation** | Add `await requireAuth(request)` as the first statement in the loader before re-enabling the route, and pass the access token to `getBackendClient` rather than constructing raw URL strings. |

---

## Files Reviewed

| File | Status |
|---|---|
| [app/routes/api.proxy.ts](FinanceFrontEnd/FinanceApp/app/routes/api.proxy.ts) | Issues found (R3) |
| [app/routes/auth.auth0.tsx](FinanceFrontEnd/FinanceApp/app/routes/auth.auth0.tsx) | No issues |
| [app/routes/auth.callback.tsx](FinanceFrontEnd/FinanceApp/app/routes/auth.callback.tsx) | No issues |
| [app/routes/auth.forbidden.tsx](FinanceFrontEnd/FinanceApp/app/routes/auth.forbidden.tsx) | No issues |
| [app/routes/auth.login.tsx](FinanceFrontEnd/FinanceApp/app/routes/auth.login.tsx) | No issues |
| [app/routes/auth.logout.tsx](FinanceFrontEnd/FinanceApp/app/routes/auth.logout.tsx) | No issues |
| [app/routes/credit-cards._index.tsx](FinanceFrontEnd/FinanceApp/app/routes/credit-cards._index.tsx) | No issues |
| [app/routes/credit-cards.statement.$id.tsx](FinanceFrontEnd/FinanceApp/app/routes/credit-cards.statement.$id.tsx) | No issues |
| [app/routes/credit-cards.tsx](FinanceFrontEnd/FinanceApp/app/routes/credit-cards.tsx) | No issues |
| [app/routes/currency-exchange-rates.tsx](FinanceFrontEnd/FinanceApp/app/routes/currency-exchange-rates.tsx) | No issues |
| [app/routes/dashboard.summary.tsx](FinanceFrontEnd/FinanceApp/app/routes/dashboard.summary.tsx) | No issues |
| [app/routes/dashboard.tsx](FinanceFrontEnd/FinanceApp/app/routes/dashboard.tsx) | No issues |
| [app/routes/funds.tsx](FinanceFrontEnd/FinanceApp/app/routes/funds.tsx) | No issues |
| [app/routes/health.ts](FinanceFrontEnd/FinanceApp/app/routes/health.ts) | No issues |
| [app/routes/incomes.tsx](FinanceFrontEnd/FinanceApp/app/routes/incomes.tsx) | No issues |
| [app/routes/investments.tsx](FinanceFrontEnd/FinanceApp/app/routes/investments.tsx) | No issues |
| [app/routes/subscriptions.tsx](FinanceFrontEnd/FinanceApp/app/routes/subscriptions.tsx) | No issues |
| [app/routes/api._index.ts](FinanceFrontEnd/FinanceApp/app/routes/api._index.ts) | No issues |
| [app/_disabled_routes/debits.tsx](FinanceFrontEnd/FinanceApp/app/_disabled_routes/debits.tsx) | No issues |
| [app/_disabled_routes/debits.monthly.tsx](FinanceFrontEnd/FinanceApp/app/_disabled_routes/debits.monthly.tsx) | No issues |
| [app/_disabled_routes/debits.annual.tsx](FinanceFrontEnd/FinanceApp/app/_disabled_routes/debits.annual.tsx) | Issues found (R7) |
| [app/services/auth/auth-session.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/auth-session.server.ts) | Issues found (R1) |
| [app/services/auth/cookie-session.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/cookie-session.server.ts) | Issues found (R1) |
| [app/services/auth/session.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/session.server.ts) | No issues |
| [app/services/auth/auth.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/auth.server.ts) | No issues |
| [app/services/auth/tokenRefresh.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/tokenRefresh.server.ts) | No issues |
| [app/middleware/logger.server.ts](FinanceFrontEnd/FinanceApp/app/middleware/logger.server.ts) | Contributes to R5 |
| [app/entry.server.tsx](FinanceFrontEnd/FinanceApp/app/entry.server.tsx) | Issues found (R5) |
| [app/root.tsx](FinanceFrontEnd/FinanceApp/app/root.tsx) | Issues found (R2, R4, R6) |
