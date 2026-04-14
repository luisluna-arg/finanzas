# Security Audit — FinanceApp Services
**Scope**: `FinanceFrontEnd/FinanceApp/app/services/`
**Date**: 2026-03-29
**Reviewer**: GitHub Copilot

---

## Summary

4 issues found. The architecture is well-structured: tokens are stored exclusively in server-side Redis, the browser cookie holds only a `serverSessionId` UUID, and JWKS-based ID token verification is in place. The critical risk is two dead-code files with hardcoded cookie signing secrets. The remaining issues are information disclosure through unconditional production logging of raw error payloads.

---

## Findings

---

### R1 — Hardcoded cookie signing secrets

| Field | Detail |
|---|---|
| **Component / File** | [app/services/auth/auth-session.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/auth-session.server.ts#L8), [app/services/auth/cookie-session.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/cookie-session.server.ts#L9) |
| **Lines** | `auth-session.server.ts` L8, `cookie-session.server.ts` L9 |
| **Impact** | The `secrets` arrays contain `"s3cr3t"` and `"your-secret-key"` respectively. An attacker with knowledge of these values can forge signed session cookies and impersonate any user. Both files are apparently dead code (neither is imported by the active auth flow), but their presence in the repository creates re-activation risk during future development. |
| **Priority** | Critical |
| **Recommendation** | Delete both files. If either is ever needed, read secrets from environment variables following the pattern established in `session.server.ts` (lines 23–34): `process.env.USER_SESSION_SECRET || process.env.SESSION_SECRET`, with a hard failure in production if the variable is absent. |

---

### R2 — Authentication errors logged unconditionally to always-on production logger

| Field | Detail |
|---|---|
| **Component / File** | [app/services/auth/auth.ts](FinanceFrontEnd/FinanceApp/app/services/auth/auth.ts#L8) |
| **Line** | L8 |
| **Impact** | When `authenticator.authenticate()` throws, the raw error is passed to `serverLogger.error()` — the always-on Pino logger, not `SafeLogger`. The error thrown by `remix-auth` on an OAuth failure may include the entire `Request` object (containing `Cookie` headers with the `__user_session` value), OAuth error codes, or OIDC `error_description` strings. All of these are emitted to production log infrastructure. |
| **Priority** | Medium |
| **Recommendation** | Log only a structured, non-sensitive summary. Drop the raw error object from the production log payload: |

```ts
export async function authenticate(request: Request) {
  try {
    return await authenticator.authenticate(AuthConstants.PROVIDER, request);
  } catch (error) {
    serverLogger.error('Authentication failed', {
      type: error instanceof Error ? error.name : 'unknown',
    });
    throw new Error('Authentication failed. Please log in again.');
  }
}
```

---

### R3 — Auth0 token-refresh error body logged unconditionally in production

| Field | Detail |
|---|---|
| **Component / File** | [app/services/auth/tokenRefresh.server.ts](FinanceFrontEnd/FinanceApp/app/services/auth/tokenRefresh.server.ts#L46) |
| **Lines** | L44–47 |
| **Impact** | When Auth0's `/oauth/token` endpoint returns a non-2xx response, `requestNewTokens` captures the full response body as text and logs it via `SafeLogger.error`. `SafeLogger.error` always runs (no `isDev` guard). Auth0 error bodies include fields like `error` and `error_description` which can reveal client configuration details (e.g. `"Client authentication failed"`, `"Unknown or invalid refresh token"`). These descriptions are emitted to production logs unconditionally. |
| **Priority** | Medium |
| **Recommendation** | Log the HTTP status only. Drop the raw body from the production log; retain it in a debug-only path if needed: |

```ts
if (!response.ok) {
  const errorBody = await response.text();
  SafeLogger.error('[tokenRefresh] Auth0 token refresh failed', { status: response.status });
  // Only in dev:
  // SafeLogger.log('[tokenRefresh] Auth0 error detail:', errorBody);
  throw new Error(`Token refresh failed: ${response.status}`);
}
```

---

### R4 — Dead fallback in REDIRECT_URI construction

| Field | Detail |
|---|---|
| **Component / File** | [app/services/auth/auth.constants.ts](FinanceFrontEnd/FinanceApp/app/services/auth/auth.constants.ts#L22) |
| **Line** | L22 |
| **Impact** | `REDIRECT_URI` is constructed as `` `${BASE_URL}/auth/callback` \|\| `http://localhost:${PORT}/auth/callback` ``. Because a template literal is always a truthy non-empty string (even if `BASE_URL` is `undefined`, the result is `"undefined/auth/callback"`), the `||` fallback is unreachable dead code. If `BASE_URL` is misconfigured in an environment, the redirect URI will silently be `"undefined/auth/callback"` rather than resolving to the intended localhost default. Auth0 will reject the mismatched `redirect_uri`, causing login breakage. |
| **Priority** | Low |
| **Recommendation** | Remove the dead fallback and rely on the existing startup guard (`if (!BASE_URL) throw new Error(...)`) to surface misconfiguration early: |

```ts
REDIRECT_URI: `${BASE_URL}/auth/callback`,
```

---

## Non-Issues (confirmed)

| Topic | Verdict |
|---|---|
| Token storage in browser | Not present — tokens live in Redis only; cookie holds a UUID session reference |
| `session.server.ts` secret management | Correctly reads from `USER_SESSION_SECRET` / `SESSION_SECRET` env vars; throws in production if absent; uses `'dev-session-secret'` only in non-production |
| JWKS-based ID token verification | `jwtVerify` with `createRemoteJWKSet` in `auth.server.ts`; 5-second race timeout prevents hanging on JWKS fetch |
| JWT expiry check in `tokenRefresh.server.ts` | `isTokenExpired` decodes without verifying — intentional for local expiry check before network call; actual verification is done by `jwtVerify` in `session.server.ts` |
| Redis URL logged at startup | Uses `SafeLogger.log` which is dev-only — safe |
| Auth0 refresh token rotation | Handled correctly — new refresh token stored if returned, old retained if not |
| `console.*` calls | None — all logging via `SafeLogger` or `serverLogger` |
