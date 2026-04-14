# Security Audit — FinanceApp Components
**Scope**: `FinanceFrontEnd/FinanceApp/app/components/`
**Date**: 2026-03-29
**Reviewer**: GitHub Copilot

---

## Summary

3 issues found. No `dangerouslySetInnerHTML` injection points, no raw `console.*` calls, no sensitive data in HTML attributes, and no direct backend access (all client-side fetches route through the `/api/proxy` BFF). The primary risk is information disclosure: API error response bodies are surfaced to users and logged unconditionally in production.

---

## Findings

---

### R1 — API error response body rendered to DOM

| Field | Detail |
|---|---|
| **Component / File** | [app/components/ui/utils/FetchTable.tsx](FinanceFrontEnd/FinanceApp/app/components/ui/utils/FetchTable.tsx) |
| **Lines** | ~108–113 (error construction), ~143 (render) |
| **Priority** | High |

On a failed fetch, `FetchTable` calls `response.text()` to capture the raw backend response body, appends it to the error message, and stores that string in React state. The state value is then rendered directly in the DOM:

```tsx
// error construction
const errorText = await response.text();
setError(new Error(`HTTP error! status: ${response.status}\n${errorText}`).message);

// render
<div>Error: {error}</div>
```

If the backend returns a verbose error payload (e.g. a 400 with financial identifiers, a 500 with a stack trace, or a validation message containing account data), the full content is displayed to any authenticated user. React's JSX encoding prevents XSS, but the information disclosure is real.

**Recommendation**: Catch fetch errors and display a generic user-facing message. Log the raw error server-side (or not at all). Do not forward backend error payloads to the client:

```tsx
} catch {
  setError('No se pudo cargar la información. Intente nuevamente.');
}
```

---

### R2 — API error response body logged unconditionally in production

| Field | Detail |
|---|---|
| **Component / File** | [app/components/data/handleRequest.ts](FinanceFrontEnd/FinanceApp/app/components/data/handleRequest.ts) |
| **Lines** | ~33–41 |
| **Priority** | Medium |

On any non-2xx response from the `/api/proxy`, `handleRequest` captures the response body as text and passes it to `SafeLogger.error`:

```ts
const errorText = await response.text();
SafeLogger.error('Request failed', { status: response.status, body: errorText });
```

Unlike `SafeLogger.log` (which is dev-only), `SafeLogger.error` always executes. The structural redaction in `SafeLogger` strips keys named `token`, `secret`, `access`, or `password`, but raw response body text arriving as the string value of `body` is not inspected for financial amounts or other sensitive content. Any financial data, account identifiers, or backend internals in a failed response body are emitted to the browser console in all environments.

**Recommendation**: Log only the HTTP status code. Drop the raw body from the log payload:

```ts
SafeLogger.error('Request failed', { status: response.status });
```

If body content is needed for debugging, gate it on `isDev`.

---

### R3 — Backend error body exposed via browser alert

| Field | Detail |
|---|---|
| **Component / File** | [app/components/ui/CreditCards/EditStatementModal.tsx](FinanceFrontEnd/FinanceApp/app/components/ui/CreditCards/EditStatementModal.tsx) |
| **Lines** | ~120, ~131, ~149, ~163 (throws), ~168 (catch) |
| **Priority** | Medium |

`EditStatementModal` performs four sequential fetch operations (statement update, transaction delete, transaction create, transaction update). Each uses the same pattern:

```ts
if (!res.ok) throw new Error(await res.text());
```

The single catch block then presents the error to the user:

```ts
catch (err) {
  alert(`Error al guardar: ${err}`);
}
```

This surfaces the raw backend response body (potentially containing financial data, internal identifiers, or validation details) in a browser `alert()` dialog. The impact is lower than R1 (a dialog, not persistent DOM text), but the information disclosure pattern is the same.

**Recommendation**: Throw a generic error from each failed branch and log the raw response body elsewhere (or drop it entirely):

```ts
if (!res.ok) {
  SafeLogger.error('Save failed', { status: res.status });
  throw new Error('Error al guardar. Intente nuevamente.');
}
```

---

## Non-Issues (confirmed)

| Topic | Verdict |
|---|---|
| `dangerouslySetInnerHTML` | Not found in any component file with API-sourced data |
| `.eval()` calls in FetchTable / PaginatedTable | These are method calls on `ConditionalClass` interface objects — not JavaScript `eval()` |
| Raw `console.*` calls | None found — all logging goes through `SafeLogger` |
| `localStorage` / `sessionStorage` | Used only in `Navigation/index.tsx` for the `'theme'` key — no financial data |
| Financial data in HTML attributes | Not present — only shadcn infrastructure `data-slot` attributes found |
| Client-side fetch auth | All `BackendUrl.toString()` calls resolve to `/api/proxy?path=…` — no direct backend access from the browser |
| `document.querySelectorAll` in PaginatedTable | Selector built from a component prop (`name`), not API-controlled data — low risk |
| `Uploader.tsx` logging | `SafeLogger.log('URL', url)` — `SafeLogger.log` is dev-only (guarded by `if (!isDev) return`) |
